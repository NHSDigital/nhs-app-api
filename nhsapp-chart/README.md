## NHS Online Chart 
This repository contains the code for the NHS Online [Helm chart](https://docs.helm.sh/developing_charts/).

## Motivation
Using Helm charts allows packaging and versioning of the Kubernetes resources required by the NHS app.

## How to use
This Helm chart is used in every deployment of the NHS app. A new chart is packaged using this [Teamcity job](https://teamcity.dev.nonlive.nhsapp.service.nhs.uk/viewType.html?buildTypeId=NHSOnline_Build_Ops_NHSAppChart) after each merge request and merge to develop, which is then tagged and stored in a chart repository.

Each chart packaged from a merge request is tagged in the format of `nhsapp-#TEAMCITYBUILDNUMBER#-MR`, and stored in the Sandbox chart repo. Any new charts packaged after a merge to the Develop branch are tagged `nhsapp-#TEAMCITYBUILDNUMBER#` and is stored in the Development chart repo.

These charts can then be used in your deployments by specifying the source repo, and build number of the chart.

## Pre-Commit Hook
You must install the pre-commit hook script in the root directory of this repository before committing any changes. This ensures that the helm charts are passing kube-score vulnerability checks (critical at the very least). You can install kube-score by running `brew install kube-score/tap/kube-score`.

Kube-score has been disabled for now as updates seem to have broken checks on Helm2, this will be reenabled on our move to Helm3.

To set up the pre-commit hook, run the following from this directory:
1. `cp pre-commit.sh .git/hooks/pre-commit`

2. `chmod 755 .git/hooks/pre-commit` will be required if you see `The '.git/hooks/pre-commit' hook was ignored because it's not set as executable.`

With the addition of 'helm lint' the structure has been changed to hold the kube-score and helm-lint scripts in a separate pre-commit.d folder and call these from pre-commit.sh.

## NOTE: Networking and Azure-NPM
Azure-NPM uses LIFO (Last In First Out) when processing rules. This means you _must_ put your default deny rules first before applying the more specific rules for controlling traffic. The default deny ingress/egress rules have been moved to the `nhsonline-infra` repository and are now applied at namespace creation time and are separate from the application chart.

For more info, check [here](https://github.com/Azure/azure-container-networking/pull/258)