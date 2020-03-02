kubectl create -f hellomq-app.yaml
kubectl create -f hellomq-ing.yaml
kubectl describe deployment hellomq-deployment

timeout /t 30
start http://localhost/hellomq/   # 注意最後要加 /   預設 guest/guest
