@echo off
setlocal

echo.
echo ============================================
echo   Configurando Git + UnityYAMLMerge
echo ============================================
echo.

REM ------------------------------------------------------------
REM Obtener la raiz del repositorio
REM ------------------------------------------------------------

set "REPO_ROOT=%~dp0.."

REM Convertir a ruta absoluta
pushd "%REPO_ROOT%"

echo Repositorio:
echo %CD%
echo.

REM ------------------------------------------------------------
REM Verificar que sea un repositorio Git
REM ------------------------------------------------------------

git rev-parse --git-dir >nul 2>&1

if errorlevel 1 (
    echo ERROR: Esta carpeta no es un repositorio Git.
    echo.
    echo Ejecuta este archivo desde un repositorio clonado.
    echo.
    popd
    pause
    exit /b 1
)

REM ------------------------------------------------------------
REM Verificar que exista .gitconfig
REM ------------------------------------------------------------

if not exist ".gitconfig" (
    echo ERROR: No se encontro:
    echo.
    echo %CD%\.gitconfig
    echo.
    popd
    pause
    exit /b 1
)

REM ------------------------------------------------------------
REM Configurar Git para incluir .gitconfig
REM ------------------------------------------------------------

echo Configurando Git...

git config --local include.path ../.gitconfig

if errorlevel 1 (
    echo.
    echo ERROR: No se pudo configurar Git.
    echo.
    popd
    pause
    exit /b 1
)

REM ------------------------------------------------------------
REM Verificar
REM ------------------------------------------------------------

echo.
echo Configuracion aplicada correctamente.
echo.

echo Configuracion actual:
echo --------------------------------------------

git config --local --get include.path

echo.
echo --------------------------------------------
echo.

echo UnityYAMLMerge quedo configurado.
echo.
echo Ya podes usar:
echo.
echo     git mergetool
echo.
echo para resolver conflictos de archivos YAML de Unity.
echo.

popd

pause