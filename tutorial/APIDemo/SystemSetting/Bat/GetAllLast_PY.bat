REM set svcName=AsMLService
net stop %svcName%


set pathsrcfolder=d:\ProgOpen\Training\Keras\CHT
REM set pathsrcfolder=d:\ProgOpen\Open\Sgx\Python\Keras\CHT

set pathtofolder=d:\App.Host\AsService\Data\Python
echo �R���Ҧ�%pathtofolder%�U���ؿ��P�ɮ�
pushd "%pathtofolder%" && (rd /s /q "%pathtofolder%" 2>nul & popd)

xcopy /i /y  %pathsrcfolder%\*Service.py %pathtofolder%
xcopy /i /y  %pathsrcfolder%\package\* %pathtofolder%\package



REM net start %svcName%
timeout 5
