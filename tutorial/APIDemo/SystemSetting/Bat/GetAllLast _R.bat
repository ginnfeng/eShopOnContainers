
set progPath=d:\ProgOpen\Open
del /y %progPath%\Sgx\Sgx.R.Lib\R\*
xcopy /i /y  %progPath%\Sgx\Sgx.R.Lib\R\* d:\App.Host\AsService\Data\R
