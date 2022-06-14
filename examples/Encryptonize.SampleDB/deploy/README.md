# Encryptonize DB Azure deployment and provisioning

## Provisioning the infrastructure

### Encryptonize service

The Encryptonize service needs to be deployed and accessible from the application. Instructions on how to quickly deploy Encryptonize to Azure are available [here](https://github.com/cybercryptio/d1-service-generic/blob/master/deployment/provision/README.md#AKS-cluster).

### SQL server

Provisioning a SQL server to Azure, can easily be done using the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) or the provided [makefile](azure/sample/makefile).

#### Azure CLI

```
az deployment group create --name "EncDBSample" --resource-group "${resourceGroup}" --template-file sql.bicep --parameters administratorLogin="${adminLogin}" administratorLoginPassword="${adminPass}" serverName="${serverName}"
```

#### Makefile

Beaware that this method only works on Linux or in WSL.

```
make deploy adminUser="${adminUser}" adminPass="${adminPass}" resource_group="${resourceGroup}" serverName="${serverName}"
```

### Sample application

Deployment of the sample application is done using a Helm template located in [EncryptonizeDBSample](EncryptonizeDBSample/).

[values.yaml](EncryptonizeDBSample/values.yaml) needs to be updated to reflect the correct values for the SQL server and the Encryptonize service. Once those values have been updated, the application can be deployed using the command:

```
helm install encryptonizedb-sample .
```