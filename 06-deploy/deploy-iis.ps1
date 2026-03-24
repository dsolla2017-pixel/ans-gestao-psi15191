# ============================================================
# Autor: Diogo Grawingholt
# Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
# Referência: https://gegodtransformacaodosdados.org
# Portfólio: https://www.diogograwingholt.com.br
# ============================================================
# REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
#
# [Infrastructure as Code] Automação de deploy via script PowerShell,
#   garantindo reprodutibilidade e eliminando erros manuais.
#
# [IIS Deployment] Publicação em Internet Information Services (IIS),
#   servidor web padrão da CAIXA para aplicações .NET.
#
# [Twelve-Factor App — Factor V] Build, Release, Run:
#   Separação clara entre build (dotnet publish) e deploy (xcopy).
# ============================================================

param(
    # Parâmetro obrigatório: caminho do site no IIS.
    # Exemplo: "Default Web Site/AnsGestao"
    [Parameter(Mandatory=$true)]
    [string]$SitePath,

    # Parâmetro opcional: ambiente de publicação.
    # Valores aceitos: "Development", "Staging", "Production"
    [string]$Environment = "Production"
)

# ── Variáveis de Configuração ──────────────────────────────────
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$BackendPath = Join-Path $ProjectRoot "03-backend\src\Caixa.Ans.Api"
$FrontendPath = Join-Path $ProjectRoot "04-frontend"
$PublishPath = Join-Path $ProjectRoot "publish"
$BackendPublish = Join-Path $PublishPath "backend"
$FrontendPublish = Join-Path $PublishPath "frontend"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " Deploy IIS — Sistema de Gestão de ANS — GEGOD/CAIXA" -ForegroundColor Cyan
Write-Host " Autor: Diogo Grawingholt" -ForegroundColor Cyan
Write-Host " https://gegodtransformacaodosdados.org" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# ── Passo 1: Limpeza do diretório de publicação ────────────────
Write-Host "[1/5] Limpando diretório de publicação..." -ForegroundColor Yellow
if (Test-Path $PublishPath) {
    Remove-Item -Recurse -Force $PublishPath
}
New-Item -ItemType Directory -Path $BackendPublish -Force | Out-Null
New-Item -ItemType Directory -Path $FrontendPublish -Force | Out-Null

# ── Passo 2: Build do Back-end (.NET 8) ────────────────────────
# dotnet publish gera os binários otimizados para produção.
# --configuration Release: otimizações do compilador habilitadas.
# --self-contained false: requer .NET Runtime no servidor (menor tamanho).
Write-Host "[2/5] Compilando back-end C#/.NET 8..." -ForegroundColor Yellow
Push-Location $BackendPath
dotnet publish -c Release -o $BackendPublish --self-contained false
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha na compilação do back-end." -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location
Write-Host "     Back-end compilado com sucesso." -ForegroundColor Green

# ── Passo 3: Build do Front-end (Angular 17 + DSC) ─────────────
# ng build --configuration production: tree-shaking, AOT, minificação.
Write-Host "[3/5] Compilando front-end Angular 17..." -ForegroundColor Yellow
Push-Location $FrontendPath
if (Test-Path "node_modules") {
    Write-Host "     node_modules encontrado, pulando npm install." -ForegroundColor Gray
} else {
    npm install
}
npx ng build --configuration production --output-path $FrontendPublish
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha na compilação do front-end." -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location
Write-Host "     Front-end compilado com sucesso." -ForegroundColor Green

# ── Passo 4: Copiar web.config para o diretório de publicação ──
# O web.config configura o módulo ASP.NET Core no IIS.
Write-Host "[4/5] Copiando web.config para publicação..." -ForegroundColor Yellow
$WebConfigSource = Join-Path $PSScriptRoot "web.config"
if (Test-Path $WebConfigSource) {
    Copy-Item $WebConfigSource -Destination $BackendPublish -Force
    Write-Host "     web.config copiado." -ForegroundColor Green
} else {
    Write-Host "     AVISO: web.config não encontrado em $WebConfigSource" -ForegroundColor Yellow
}

# ── Passo 5: Deploy para o IIS ──────────────────────────────────
# Copia os artefatos para o diretório físico do site no IIS.
Write-Host "[5/5] Publicando no IIS: $SitePath" -ForegroundColor Yellow

# Verifica se o módulo IIS está disponível
if (Get-Module -ListAvailable -Name WebAdministration) {
    Import-Module WebAdministration

    # Para o Application Pool antes do deploy (evita arquivos em uso)
    $appPool = (Get-WebApplication -Name (Split-Path $SitePath -Leaf) -ErrorAction SilentlyContinue)
    if ($appPool) {
        Stop-WebAppPool -Name $appPool.applicationPool -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    }

    # Copia os arquivos para o diretório do IIS
    $iisPhysicalPath = (Get-WebFilePath "IIS:\Sites\$SitePath" -ErrorAction SilentlyContinue)
    if ($iisPhysicalPath) {
        Copy-Item -Path "$BackendPublish\*" -Destination $iisPhysicalPath -Recurse -Force
        Write-Host "     Arquivos copiados para $iisPhysicalPath" -ForegroundColor Green
    } else {
        Write-Host "     AVISO: Caminho físico do IIS não encontrado." -ForegroundColor Yellow
        Write-Host "     Copie manualmente de: $BackendPublish" -ForegroundColor Yellow
    }

    # Reinicia o Application Pool
    if ($appPool) {
        Start-WebAppPool -Name $appPool.applicationPool -ErrorAction SilentlyContinue
    }
} else {
    Write-Host "     Módulo WebAdministration não disponível." -ForegroundColor Yellow
    Write-Host "     Copie manualmente os arquivos de:" -ForegroundColor Yellow
    Write-Host "       Back-end: $BackendPublish" -ForegroundColor White
    Write-Host "       Front-end: $FrontendPublish" -ForegroundColor White
}

# ── Resumo Final ────────────────────────────────────────────────
Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host " Deploy concluído com sucesso!" -ForegroundColor Green
Write-Host " Ambiente: $Environment" -ForegroundColor Green
Write-Host " Back-end: $BackendPublish" -ForegroundColor Green
Write-Host " Front-end: $FrontendPublish" -ForegroundColor Green
Write-Host "" -ForegroundColor Green
Write-Host " Documentação: https://gegodtransformacaodosdados.org" -ForegroundColor Cyan
Write-Host " Portfólio: https://www.diogograwingholt.com.br" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Green
