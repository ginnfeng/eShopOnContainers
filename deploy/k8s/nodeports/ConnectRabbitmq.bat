kubectl create -f rabbitmq-ingress.yaml
kubectl describe svc rabbitmq-admin-service
kubectl describe ing rabbitmq-admin-ingress

echo "Waiting for the eShop-rabbitmq management website to open ..."
timeout /t 10
start http://localhost/mqadmin/  # 注意最後要加 /   預設 guest/guest
