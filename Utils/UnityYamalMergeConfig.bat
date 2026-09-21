@echo off
setlocal EnableDelayedExpansion

REM ============================================================
REM UnityYAMLMerge launcher
REM
REM Busca automaticamente UnityYAMLMerge.exe sin importar
REM en que disco este instalado Unity.
REM
REM La version de Unity se obtiene de:
REM ProjectSettings/ProjectVersion.txt
REM ============================================================

REM ------------------------------------------------------------
REM 1. Obtener raiz del proyecto
REM ------------------------------------------------------------

set "REPO_ROOT=%~dp0.."
set "VERSION_FILE=%REPO_ROOT%\ProjectSettings\ProjectVersion.txt"

if not exist "%VERSION_FILE%" (
    echo.
    echo ERROR: No se encontro ProjectVersion.txt
    echo.
    echo "%VERSION_FILE%"
    echo.
    exit /b 1
)

REM ------------------------------------------------------------
REM 2. Obtener version de Unity
REM ------------------------------------------------------------

set "UNITY_VERSION="

for /f "tokens=2 delims=: " %%V in ('findstr /B "m_EditorVersion:" "%VERSION_FILE%"') do (
    set "UNITY_VERSION=%%V"
)

if not defined UNITY_VERSION (
    echo.
    echo ERROR: No se pudo determinar la version de Unity.
    echo.
    exit /b 1
)

echo.
echo ============================================
echo Unity version: %UNITY_VERSION%
echo ============================================
echo.

REM ------------------------------------------------------------
REM 3. Buscar UnityYAMLMerge.exe
REM ------------------------------------------------------------

set "UNITY_MERGE="

echo Buscando UnityYAMLMerge.exe...

REM ============================================================
REM Buscar en todos los discos disponibles.
REM ============================================================

for %%D in (C D E F G H I J K L M N O P Q R S T U V W X Y Z) do (

    REM --------------------------------------------------------
    REM Unity Hub - ubicacion habitual
    REM --------------------------------------------------------

    if exist "%%D:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe" (
        set "UNITY_MERGE=%%D:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe"
        goto :FOUND
    )

    REM --------------------------------------------------------
    REM Program Files x86
    REM --------------------------------------------------------

    if exist "%%D:\Program Files (x86)\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe" (
        set "UNITY_MERGE=%%D:\Program Files (x86)\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe"
        goto :FOUND
    )

    REM --------------------------------------------------------
    REM Instalacion personalizada comun
    REM --------------------------------------------------------

    if exist "%%D:\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe" (
        set "UNITY_MERGE=%%D:\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe"
        goto :FOUND
    )

    if exist "%%D:\Unity\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe" (
        set "UNITY_MERGE=%%D:\Unity\Editor\%UNITY_VERSION%\Editor\Data\Tools\UnityYAMLMerge.exe"
        goto :FOUND
    )

)

REM ============================================================
REM 4. Busqueda amplia
REM
REM Si Unity fue instalado en una ubicacion completamente
REM personalizada, buscamos UnityYAMLMerge.exe en cada disco.
REM ============================================================

echo.
echo No se encontro en las ubicaciones habituales.
echo.
echo Buscando en los discos disponibles...
echo Esto puede tardar un poco.
echo.

for %%D in (C D E F G H I J K L M N O P Q R S T U V W X Y Z) do (

    if exist "%%D:\" (

        for /f "delims=" %%F in ('dir "%%D:\UnityYAMLMerge.exe" /s /b /a-d 2^>nul') do (

            echo %%F | findstr /i "\\%UNITY_VERSION%\\.*UnityYAMLMerge.exe$" >nul

            if !errorlevel! equ 0 (
                set "UNITY_MERGE=%%F"
                goto :FOUND
            )

        )

    )

)

REM ------------------------------------------------------------
REM 5. No encontrado
REM ------------------------------------------------------------

echo.
echo ============================================================
echo ERROR: No se encontro UnityYAMLMerge.exe
echo ============================================================
echo.
echo Version requerida:
echo %UNITY_VERSION%
echo.
echo Asegurate de tener instalada esa version de Unity.
echo.
exit /b 1

REM ------------------------------------------------------------
REM 6. Encontrado
REM ------------------------------------------------------------

:FOUND

echo.
echo ============================================================
echo UnityYAMLMerge encontrado:
echo.
echo %UNITY_MERGE%
echo ============================================================
echo.

REM ------------------------------------------------------------
REM 7. Ejecutar UnityYAMLMerge
REM
REM Git pasa:
REM %1 = BASE
REM %2 = REMOTE
REM %3 = LOCAL
REM %4 = MERGED
REM ------------------------------------------------------------

"%UNITY_MERGE%" merge -p "%~1" "%~2" "%~3" "%~4"

set "EXIT_CODE=%ERRORLEVEL%"

exit /b %EXIT_CODE%