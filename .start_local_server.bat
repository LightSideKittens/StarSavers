@echo off
REM Переключаемся на директорию скрипта
cd /d %~dp0

REM Создаем папку ServerData, если её нет
if not exist Server (
    mkdir Server
)

REM Переключаемся на директорию ServerData
cd Server

REM Запускаем локальный сервер
python -m http.server 8000

pause
