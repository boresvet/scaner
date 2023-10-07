:loop
start Sick-test.exe ...
timeout /t 1200 >null
taskkill /f /im Sick-test.exe >nul
goto loop