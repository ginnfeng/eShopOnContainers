#https://gitee.com/pikaih/eShopOnContainers/blob/master/k8s/deploy-nodeports.ps1

kubectl apply -f .\nodeports\rabbitmq-admin.yaml
kubectl apply -f .\nodeports\sql-services.yaml