
tree
del /f /q charts\*.*

helm install --dry-run apidemo --values inf.yaml  . --dependency-update --namespace dev > a.txt

a.txt