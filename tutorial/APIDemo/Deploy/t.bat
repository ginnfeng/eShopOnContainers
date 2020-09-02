
tree
del /f /q .\main\charts\*.*

helm install --dry-run --debug apidemo --values .\main\inf.yaml  .\main --dependency-update --namespace dev > .\main\a.txt
.\main\a.txt 