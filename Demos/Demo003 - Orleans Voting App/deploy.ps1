$resourceGroup = "globalazure2025votingapp"
$location = "westus"
$clusterName = "globalazure2025votingapp"
$containerRegistry = "globalazure2025dncvotingapp"

$acrLoginServer = $(az acr show --name $containerRegistry --resource-group $resourceGroup --query loginServer).Trim('"')
az acr login --name $containerRegistry

docker build . -t $acrLoginServer/globalazure2025votingapp &&
docker push $acrLoginServer/globalazure2025votingapp &&
kubectl apply -f ./deployment.yaml &&
kubectl rollout restart deployment/globalazure2025votingapp
