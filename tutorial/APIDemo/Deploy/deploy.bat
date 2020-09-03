helm uninstall apidemo
tree
del /f /q ./main/charts/*.*
helm install apidemo --values ./main/inf.yaml  ./main --dependency-update --namespace dev
REM helm install -f inf.yaml --dry-run apidemo . --dependency-update --namespace dev >> a.txt

helm install --dry-run --debug apidemo --values .\main\inf.yaml  .\main --dependency-update --namespace dev > .\main\a.txt
.\main\a.txt 

kubectl get all