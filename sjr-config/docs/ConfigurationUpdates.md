# Configuration Updates

## Prerequisites

1. Ensure you have sqlite 3 and python 2.7.18 installed and added to your path.
2. Join the #sjr-third-party-updates Slack channel to keep updated on required SJR config updates.
3. Unless you are in the situation where you are preparing an SJR config change in `develop` ready to for a cut of `develop` to be taken for a release, find the release branch that is currently live and branch off it to make your changes.  Your PR will be merged into the release branch. 

## Making Configuration Updates

### GP Info update

1. Acquire the latest weekly GP info csv file. This is emailed to the NHS App mailbox every Sunday - a member of the T2 or T3 support team will be able to access the latest.

2. Update `sjr-config/utils/gpinfocreation/data/full_gpinfo.csv` and/or `sjr-config/utils/gpinfocreation/data/excluded-practices.csv`.

3. From the root directory of nhsapp repo run
	```bash
	sjr-config/utils/gpinfocreation/src/process_gpinfo.sh
	```
	to update `gpinfo.csv`

### eConsult update

1. The weekly eConsult updates are raised through Cherwell service requests each week (search for "NHS App Supplier Standard Change Request"). They are provided as an xlsx file, with additions and deletions on separate worksheets. Contact a member of the Tier 2 support team to forward the file if you do not have access to Cherwell.

2. Update `sjr-config/utils/rulecreation/econsult.csv` with the practice changes. For removals, just delete the row where you find the ODS code to be removed. Additions should be added as entries to ccg "Other" at the end of the file. There is no need to match to a particular CCG. You can do this by first saving the the additions sheet as a csv e.g. `200727-additions.csv`. Then run

	```bash
	awk -F, '{print ",,,Other,,,"$2","$1","}' 200727-additions.csv > 200727-additions-reformatted.csv
	```
	to get a file in the appropriate format. Append the additional rows to `econsult.csv`.

3. Run the following to update the set of eConsult yaml files

	```bash
	python sjr-config/utils/rulecreation/processolc.py
	```

### PKB update
1. The PKB updates are posted in the #sjr-third-party-updates Slack channel by Matt Deaves or Stewart Fishman.
2. Update `sjr-config/configurations/Journeys/patientsKnowBest/*.yaml` with the practice changes - there are files for each CCG. Note that the there are different brands of PKB, and what is enabled is not necessarily consistent at a brand level, but at a CCG level.

### Engage update

1. The Engage updates are posted in the #sjr-third-party-updates Slack channel by Mark Goldthorpe.

2. Update `sjr-config/utils/rulecreation/engage.csv` with the practice changes. For removals, just delete the row where you find the ODS code to be removed. Additions should be added to the end of the file.

3. Run the following to update the Engage yaml file

	```bash
	python sjr-config/utils/rulecreation/processengage.py
	```

## Getting configuration changes released

1. Create a PR with the required changes. Submit for review and merge into the release branch (or `develop` if you did not need to create a release branch).

2. Tag the nhsapp repo in your release branch with the new patch version e.g.

	```bash
	git tag -a v1.36.5 -m "v1.36.5 SJR Updates"
	```
	and push the tag e.g.

	```bash
	git push origin v1.36.5
	```
	or much easier is to use the Azure DevOps UI for the repo.

3. Run the nhsapp CI pipeline [https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=4](https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=4) with the following options:

	*Branch/tag* set to your new tag of `nhsapp`.
	*Stages to run* with Android Client, iOS Client and Generate Code Coverage *unset*

4. Run the acr image sync job [https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=75](https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=75) with *Image to sync* set to `all`. The *Branch/tag* dropdown can be left on `develop`.

5. Create a new Release from your Tag using the NHSApp Release Pipeline in ADO [https://dev.azure.com/nhsapp/NHS%20App/_release?_a=releases&view=mine&definitionId=4](https://dev.azure.com/nhsapp/NHS%20App/_release?_a=releases&view=mine&definitionId=4) 

6. Progress the release through to staging, and give it a quick test there to ensure it looks ok. You may need to seek approval to deploy to staging. When you are happy, notify Ops that you are deploying to cold and get approval for that step. Test in cold with a live account [https://www-cold.production.nhsapp.service.nhs.uk/](https://www-cold.production.nhsapp.service.nhs.uk/). When you are happy notify Ops so that the release may be switched live. If you deploy to cold and don't progress to switching live, also let Ops know so they can clear the release down to save space.

7. Create the new file of eConsult live surgeries:

	```bash
	  awk -F, '{print $8","$7}' sjr-config/utils/rulecreation/econsult.csv > sjr-config/utils/rulecreation/200729-practices-live.csv
	```

	and add to the [eConsult SharePoint folder](https://hscic365.sharepoint.com/:f:/r/sites/eConsult/Shared%20Documents/eConsult%20Practice%20Management?csf=1&web=1&e=xfDYrk). Do this even if there were no changes to eConsult for the release. 
	
	Email the file to recipients dave.evans@webgp.com and liz.jones@webgp.com with subject 'NHS App config'.

8. Add the latest sjr-config/utils/rulecreation/engage.csv to the [Engage SharePoint folder](https://hscic365.sharepoint.com/sites/EngageHealthSystemsLtd/Shared%20Documents/Forms/AllItems.aspx?csf=1&web=1&e=097t7s&cid=1e0bd1d8%2D3964%2D4bba%2D80e2%2Ddc7b3b6abca1&RootFolder=%2Fsites%2FEngageHealthSystemsLtd%2FShared%20Documents%2FEngage%20Practice%20Management&FolderCTID=0x0120009F236CF489C42E4E8C5F68FA5FC260FE) and rename the csv file to the format `YYMMDD-practices-live.csv`. Do this even if there were no changes to Engage for the release.

9. If you updated a release branch then cherry pick the update back into `develop`. This will ensure the next release cut is up-to-date with SJR.
