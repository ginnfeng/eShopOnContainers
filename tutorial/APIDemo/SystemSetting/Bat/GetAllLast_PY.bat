REM set svcName=AsMLService
net stop %svcName%


set pathsrcfolder=d:\ProgOpen\Training\Keras\CHT
REM set pathsrcfolder=d:\ProgOpen\Open\Sgx\Python\Keras\CHT

set pathtofolder=d:\App.Host\AsService\Data\Python
echo 刪除所有%pathtofolder%下的目錄與檔案
pushd "%pathtofolder%" && (rd /s /q "%pathtofolder%" 2>nul & popd)

xcopy /i /y  %pathsrcfolder%\*Service.py %pathtofolder%
xcopy /i /y  %pathsrcfolder%\package\* %pathtofolder%\package



REM net start %svcName%
timeout 5
