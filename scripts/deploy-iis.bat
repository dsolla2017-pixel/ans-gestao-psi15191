@echo off
REM ============================================================
REM Autor: Solla, Diogo
REM Projeto: Sistema de Gestao de ANS - CAIXA
REM ============================================================
REM GLOSSARIO PARA LEIGO:
REM Este script realiza o deploy da aplicacao em um servidor IIS.
REM O caminho do servidor IIS deve ser informado como parametro.
REM Exemplo: deploy-iis.bat C:\inetpub\wwwroot\ans-gestao
REM ============================================================

echo ============================================
echo  Deploy - Sistema de Gestao de ANS
echo  PSI 15191 - CAIXA Economica Federal
echo ============================================
echo.

REM Validar parametro
if "%~1"=="" (
    echo USO: deploy-iis.bat [CAMINHO_IIS]
    echo Exemplo: deploy-iis.bat C:\inetpub\wwwroot\ans-gestao
    pause
    exit /b 1
)

set IIS_PATH=%~1

REM Build do back-end
echo [1/4] Compilando back-end (Release)...
cd /d "%~dp0..\backend\src\Caixa.Ans.Api"
dotnet publish -c Release -o "%~dp0..\publish\backend"
if %errorlevel% neq 0 (
    echo ERRO: Falha na compilacao do back-end.
    pause
    exit /b 1
)
echo Back-end compilado com sucesso.
echo.

REM Build do front-end
echo [2/4] Compilando front-end (Production)...
cd /d "%~dp0..\frontend"
call npx ng build --configuration=production --output-path="%~dp0..\publish\frontend"
if %errorlevel% neq 0 (
    echo ERRO: Falha na compilacao do front-end.
    pause
    exit /b 1
)
echo Front-end compilado com sucesso.
echo.

REM Copiar arquivos para IIS
echo [3/4] Copiando arquivos para %IIS_PATH%...
if not exist "%IIS_PATH%" mkdir "%IIS_PATH%"
xcopy /E /Y /I "%~dp0..\publish\backend\*" "%IIS_PATH%\api\"
xcopy /E /Y /I "%~dp0..\publish\frontend\*" "%IIS_PATH%\wwwroot\"
echo Arquivos copiados.
echo.

REM Copiar web.config
echo [4/4] Configurando IIS...
copy /Y "%~dp0web.config" "%IIS_PATH%\web.config"
echo.

echo ============================================
echo  Deploy concluido com sucesso!
echo  Caminho: %IIS_PATH%
echo ============================================
echo.
echo PROXIMOS PASSOS:
echo 1. Abra o IIS Manager
echo 2. Crie um Application Pool (.NET CLR v4.0, Pipeline Integrado)
echo 3. Aponte o site para: %IIS_PATH%
echo 4. Configure a connection string no appsettings.json
echo 5. Reinicie o Application Pool
echo.
pause
