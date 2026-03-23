# ============================================================
# Autor: Diogo Grawingholt
# Projeto: Sistema de Gestao de ANS - CAIXA
# ============================================================
# GLOSSARIO PARA LEIGO:
# Este script verifica se todos os arquivos de codigo possuem
# o header de autoria padrao. Gera um relatorio de conformidade.
# ============================================================

import os
import sys
from datetime import datetime

AUTHOR_PATTERN = "Diogo Grawingholt"
PROJECT_PATTERN = "Sistema de Gestao de ANS"
EXTENSIONS = {'.cs', '.ts', '.sql', '.py', '.bat', '.ps1'}
EXCLUDE_DIRS = {'node_modules', 'bin', 'obj', '.git', 'dist', 'publish'}

def check_file(filepath):
    """Verifica se o arquivo possui header de autoria."""
    try:
        with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
            content = f.read(500)  # Ler apenas os primeiros 500 chars
            has_author = AUTHOR_PATTERN.lower() in content.lower()
            has_project = PROJECT_PATTERN.lower() in content.lower()
            return has_author and has_project
    except Exception:
        return False

def scan_directory(root_dir):
    """Escaneia o diretorio e verifica todos os arquivos."""
    results = []
    for dirpath, dirnames, filenames in os.walk(root_dir):
        # Excluir diretorios irrelevantes
        dirnames[:] = [d for d in dirnames if d not in EXCLUDE_DIRS]
        
        for filename in filenames:
            ext = os.path.splitext(filename)[1].lower()
            if ext in EXTENSIONS:
                filepath = os.path.join(dirpath, filename)
                relpath = os.path.relpath(filepath, root_dir)
                has_header = check_file(filepath)
                results.append({
                    'file': relpath,
                    'has_header': has_header,
                    'extension': ext
                })
    return results

def generate_report(results, root_dir):
    """Gera relatorio de auditoria."""
    total = len(results)
    compliant = sum(1 for r in results if r['has_header'])
    non_compliant = total - compliant
    
    print("=" * 60)
    print("  RELATORIO DE AUDITORIA DE AUTORIA")
    print(f"  Projeto: {PROJECT_PATTERN}")
    print(f"  Autor: {AUTHOR_PATTERN}")
    print(f"  Data: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"  Diretorio: {root_dir}")
    print("=" * 60)
    print()
    print(f"  Total de arquivos analisados: {total}")
    print(f"  Conformes (com header):       {compliant} ({compliant/total*100:.1f}%)" if total > 0 else "")
    print(f"  Nao conformes:                {non_compliant}")
    print()
    
    if non_compliant > 0:
        print("  ARQUIVOS SEM HEADER DE AUTORIA:")
        print("  " + "-" * 50)
        for r in results:
            if not r['has_header']:
                print(f"  [X] {r['file']}")
        print()
    
    print("  ARQUIVOS CONFORMES:")
    print("  " + "-" * 50)
    for r in results:
        if r['has_header']:
            print(f"  [OK] {r['file']}")
    
    print()
    print("=" * 60)
    if non_compliant == 0:
        print("  RESULTADO: TODOS OS ARQUIVOS ESTAO CONFORMES!")
    else:
        print(f"  RESULTADO: {non_compliant} ARQUIVO(S) PRECISAM DE CORRECAO")
    print("=" * 60)
    
    return non_compliant == 0

if __name__ == '__main__':
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    if len(sys.argv) > 1:
        root = sys.argv[1]
    
    results = scan_directory(root)
    success = generate_report(results, root)
    sys.exit(0 if success else 1)
