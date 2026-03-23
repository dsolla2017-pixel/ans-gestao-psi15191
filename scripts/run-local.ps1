# ============================================================
# Autor: Solla, Diogo
# Projeto: Sistema de Gestao de ANS - CAIXA
# ============================================================
# GLOSSARIO PARA LEIGO:
# Este script PowerShell inicia a aplicacao localmente.
# Alternativa ao .bat para ambientes que preferem PowerShell.
# ============================================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host " Sistema de Gestao de ANS - Execucao Local" -ForegroundColor Cyan
Write-Host " PSI 15191 - CAIXA Economica Federal" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Verificar pre-requisitos
Write-Host "[1/5] Verificando pre-requisitos..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "  ERRO: .NET SDK nao encontrado." -ForegroundColor Red
    exit 1
}

try {
    $nodeVersion = node --version
    Write-Host "  Node.js: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "  ERRO: Node.js nao encontrado." -ForegroundColor Red
    exit 1
}

# Restaurar pacotes
Write-Host ""
Write-Host "[2/5] Restaurando pacotes do back-end..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\backend\src\Caixa.Ans.Api"
dotnet restore

Write-Host ""
Write-Host "[3/5] Instalando dependencias do front-end..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\frontend"
npm install

# Iniciar back-end
Write-Host ""
Write-Host "[4/5] Iniciando back-end..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\backend\src\Caixa.Ans.Api"
$backendJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run --urls=http://localhost:5000
}
Start-Sleep -Seconds 5
Write-Host "  Back-end: http://localhost:5000" -ForegroundColor Green
Write-Host "  Swagger:  http://localhost:5000/swagger" -ForegroundColor Green

# Iniciar front-end
Write-Host ""
Write-Host "[5/5] Iniciando front-end..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\frontend"
$frontendJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    npx ng serve --port 4200
}
Start-Sleep -Seconds 3
Write-Host "  Front-end: http://localhost:4200" -ForegroundColor Green

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host " Aplicacao iniciada com sucesso!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Pressione Ctrl+C para encerrar..." -ForegroundColor Gray

# Aguardar
try {
    Wait-Job $backendJob, $frontendJob
} finally {
    Stop-Job $backendJob, $frontendJob -ErrorAction SilentlyContinue
    Remove-Job $backendJob, $frontendJob -ErrorAction SilentlyContinue
    Write-Host "Servidores encerrados." -ForegroundColor Yellow
}
