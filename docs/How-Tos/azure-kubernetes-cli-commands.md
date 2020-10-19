# Install Azure Cli, kubectl and stern
* https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos?view=azure-cli-latest, 
* https://kubernetes.io/docs/tasks/tools/install-kubectl/
* https://github.com/wercker/stern

Or, if you use homebrew, run the following: \
```brew update && brew install azure-cli kubectl stern```

# Azure CLI (az)
[Azure CLI](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest) is a set of commands used to create and manage Azure resources

Run (This will open a browser, login using your @hscic.gov.uk account): \
```az login```

# Azure subscriptions:

* Nonlive: sandbox/dev/scratch
* Live:  staging/production

## Get credentials for the Dev Azure CLI (Nonlive):

Set active account for Nonlive: \
```az account set --subscription="4bbd6a1f-80a5-485b-a0bb-32c5b6e35c09"```

Get credentials: \
```az aks get-credentials --resource-group nhsapp-devuks1 --name nhsapp-devuks1 --overwrite-existing```

If you get permission errors here - make sure to run `az logout` and then login again to refresh your permissions

## Get credentials for the Staging Azure CLI (Live):

Set active account for Live: \
```az account set --subscription="47e498b8-ad52-4421-b6d0-d7f9988429df"```

Get credentials: \
```az aks get-credentials --resource-group nhsapp-staginguks1 --name nhsapp-staginguks1 --overwrite-existing```

# Kubernetes command-line tool (kubectl)
The [Kubernetes command-line tool](https://kubernetes.io/docs/tasks/tools/install-kubectl/) allows you to run commands against Kubernetes clusters.

## Switching kubectl context for different environments

Dev: \
```kubectl config use-context nhsapp-devuks1```

Staging: \
```kubectl config use-context nhsapp-staginguks1```

Production: \
```kubectl config use-context nhsapp-productionuks1```

## Using kubectl

List Pods for an environment: \
```kubectl -n preview get pods```

Describe pods: \
```kubectl -n preview describe pod nhsapp-users-develop-56b799cff-75pcb```

## Looking at secrets on kubectl

List secrets in an environment: \
```kubectl get secrets -n preview```

View a base64 encoded secret in an environment: \
```kubectl get secret -n preview notification-hub-shared-access-key-develop -o yaml```

If you find that the secrets are cached/outdated, you can delete the store running the following, and then re-deploy to the desired envirnment \
```kubectl delete secrets -n preview --all```

# stern :
[Stern](https://github.com/wercker/stern) allows you to tail multiple pods on Kubernetes and multiple containers within the pod. Each result is color coded for quicker debugging

Tailing logs using Stern \
```stern ".*" -n preview```


# Helpful Alias :

```
alias nonlive=“az account set --subscription=‘4bbd6a1f-80a5-485b-a0bb-32c5b6e35c09’”
alias live=“az account set --subscription=‘47e498b8-ad52-4421-b6d0-d7f9988429df’”
alias dev=“kubectl config use-context nhsapp-devuks1”
alias staging=“kubectl config use-context nhsapp-staginguks1"
alias prod=“kubectl config use-context nhsapp-productionuks1”
```