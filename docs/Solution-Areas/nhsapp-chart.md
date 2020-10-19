## NHS Online Chart 

This repository contains the code for the NHS Online [Helm chart](https://docs.helm.sh/developing_charts/).

## Motivation

Using Helm charts allows us to define how multiple containers and Kubernetes resources are deployed as one into a given environment.

## Component parts

Whilst the helm chart defines how the NHS App is deployed, it doesn't itself contain anything deployable. Upon deploying the chart kubernetes takes the following component parts and puts them together as describe by the chart:
 * Docker images - from Azure Container Registry
 * Environment variables - defined in variable files
 * [Secrets - stored in Azure Keyvault](./nhsapp-chart/secrets-provider.md)
