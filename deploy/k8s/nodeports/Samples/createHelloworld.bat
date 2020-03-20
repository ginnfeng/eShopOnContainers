kubectl create -f helloworld-app.yaml
kubectl create -f helloworld-ing.yaml
kubectl describe deployment helloworld-deployment

timeout /t 5

start http://localhost/helloworld
rem start microsoft-edge:http://localhost/helloworld
