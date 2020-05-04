net stop AsService

set progPath=d:\ProgOpen\Open

set workingPath=d:\App.Host\AsService
set pathtofolder=%workingPath%\Packs
echo 刪除所有%pathtofolder%下的目錄與檔案
pushd "%pathtofolder%" && (rd /s /q "%pathtofolder%" 2>nul & popd)

REM xcopy /i /y  %progPath%\Sgx\Sgx.R.Lib\R\* d:\App.Host\AsService\Data\R

xcopy /i /y  %progPath%\Gov\AppScheduler.Pack\bin\Debug\*.dll %workingPath%\Packs\AppScheduler.Pack
xcopy /i /y  %progPath%\Gov\AppScheduler.Pack\bin\Debug\*.config %workingPath%\Packs\AppScheduler.Pack

xcopy /i /y  %progPath%\Gov\AppZoo.Pack\bin\Debug\*.dll %workingPath%\Packs\AppZoo.Pack

xcopy /i /y  %progPath%\Sgx\Sgx.App.Pack\bin\Debug\*.dll %workingPath%\Packs\Sgx.App.Pack
xcopy /i /y  %progPath%\Sgx\Sgx.App.Pack\bin\Debug\*.config %workingPath%\Packs\Sgx.App.Pack

del %workingPath%\*.dll
del %workingPath%\*.pdb
del %workingPath%\*.config


xcopy /i /y  %progPath%\Gov\App.AsService\bin\Debug\*.dll %workingPath%
xcopy /i /y  %progPath%\Gov\App.AsService\bin\Debug\*.pdb %workingPath%
xcopy /i /y  %progPath%\Gov\App.AsService\bin\Debug\*.config %workingPath%
xcopy /i /y  %progPath%\Gov\App.AsService\bin\Debug\AsService.exe %workingPath%

net start AsService
