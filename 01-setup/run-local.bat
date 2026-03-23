@echo off
REM ============================================================
REM Autor: Diogo Grawingholt
REM Projeto: Sistema de Gestao de ANS - CAIXA
REM ============================================================
REM GLOSSARIO PARA LEIGO:
REM Este script inicia a aplicacao localmente para desenvolvimento.
REM Pre-requisitos: .NET 8 SDK, Node.js 18+, Angular CLI.
REM ============================================================

echo ============================================
echo  Sistema de Gestao de ANS - Execucao Local
echo  PSI 15191 - CAIXA Economica Federal
echo ============================================
echo.

REM Verificar pre-requisitos
echo [1/5] Verificando pre-requisitos...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERRO: .NET SDK nao encontrado. Instale o .NET 8 SDK.
    echo Download: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERRO: Node.js nao encontrado. Instale o Node.js 18+.
    echo Download: https://nodejs.org/
    pause
    exit /b 1
)

echo Pre-requisitos OK!
echo.

REM Restaurar pacotes do back-end
echo [2/5] Restaurando pacotes do back-end...
cd /d "%~dp0..\backend\src\Caixa.Ans.Api"
dotnet restore
if %errorlevel% neq 0 (
    echo ERRO: Falha ao restaurar pacotes do back-end.
    pause
    exit /b 1
)
echo.

REM Instalar dependencias do front-end
echo [3/5] Instalando dependencias do front-end...
cd /d "%~dp0..\frontend"
call npm install
if %errorlevel% neq 0 (
    echo ERRO: Falha ao instalar dependencias do front-end.
    pause
    exit /b 1
)
echo.

REM Iniciar back-end em background
echo [4/5] Iniciando back-end (porta 5000)...
cd /d "%~dp0..\backend\src\Caixa.Ans.Api"
start "ANS-Backend" cmd /c "dotnet run --urls=http://localhost:5000"
timeout /t 5 /nobreak >nul
echo Back-end iniciado em http://localhost:5000
echo Swagger disponivel em http://localhost:5000/swagger
echo.

REM Iniciar front-end
echo [5/5] Iniciando front-end (porta 4200)...
cd /d "%~dp0..\frontend"
start "ANS-Frontend" cmd /c "npx ng serve --port 4200 --open"
timeout /t 3 /nobreak >nul
echo Front-end iniciado em http://localhost:4200
echo.

echo ============================================
echo  Aplicacao iniciada com sucesso!
echo  Back-end:  http://localhost:5000
echo  Swagger:   http://localhost:5000/swagger
echo  Front-end: http://localhost:4200
echo ============================================
echo.
echo Pressione qualquer tecla para encerrar ambos os servidores...
pause >nul

REM Encerrar processos
taskkill /FI "WINDOWTITLE eq ANS-Backend" /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq ANS-Frontend" /F >nul 2>&1
echo Servidores encerrados.
