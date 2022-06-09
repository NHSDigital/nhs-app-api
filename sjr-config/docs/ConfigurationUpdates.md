# Configuration Updates

## Prerequisites

1. Ensure you have sqlite 3 and python 2.7.18 installed and added to your path.
2. Join the #sjr-third-party-updates Slack channel to keep updated on required SJR config updates.
3. Unless you are in the situation where you are preparing an SJR config change in `develop` ready to for a cut of `develop` to be taken for a release, find the release branch that is currently live and branch off it to make your changes.  Your PR will be merged into the release branch. 

## Making Configuration Updates

### GP Info update

1. Acquire the latest weekly GP info csv file. This is emailed to the NHS App mailbox every Sunday - a member of the T2 or T3 support team will be able to access the latest. Rename the attachment from `gpinfo.csv` to `full_gpinfo.csv`.

2. Replace `sjr-config/utils/gpinfocreation/data/full_gpinfo.csv` with the renamed emailed attachment. This is the input file for the bash script in step 3.

3. From the root directory of nhsapp repo run
	```bash
	sjr-config/utils/gpinfocreation/src/process_gpinfo.sh
	```
	to update `sjr-config/configurations/gpinfo.csv`

4. Check that `sjr-config/configurations/gpinfo.csv` has been updated.

### Online consultation updates (eConsult and accuRx)

1. The eConsult updates are raised through Service Now service requests (search for "NHS App Supplier Standard Change Request"), usually on a weekly basis. A member of the T2 support team should then post the file in the sjr-third-party-updates Slack channel. They are provided as an xlsx file, with additions and deletions on separate worksheets. Contact a member of the T3 support team to forward the file if you do not have access to Service Now.

2. Update `sjr-config/utils/rulecreation/econsult.csv` with the practice changes. For removals, just delete the row where you find the ODS code to be removed. Additions should be added as entries to ccg "Other" at the end of the file. There is no need to match to a particular CCG. You can do this by first saving the the additions sheet as a csv e.g. `220727-additions.csv`. Then run

	```bash
	awk -F, '{print ",,,Other,,,"$2","$1","}' 220727-additions.csv > 220727-additions-reformatted.csv
	```
	to get a file in the appropriate format. Append the additional rows to `econsult.csv`.
3. accuRx updates are provided from Matt Deaves. Note that these updates may also involve related changes to the eConsult config. Update `sjr-config/utils/rulecreation/accurx.csv` with the practice changes required (both additions and deletions). 
4. If we have been directed to turn off on olc provider for a given ODS code in conjunction with enabling the other olc provider, update `sjr-config/utils/rulecreation/olcoverrides.csv` so that we know which should be enabled, the reason for this and the date the decision was made.
5. Once both `econsult.csv` and `accurx.csv` are updated, run 
	```bash
	python sjr-config/utils/rulecreation/checkolcclash.py
	```
	This is to check if we have both OLC providers enabled for a given ODS code.  If there is a clash, what happens next depends on whether a "winner" is output (based on data from the `olcoverrides.csv` file). Where a winner is already decided, the amendment for the losing provider should be undone. If the winner has not yet been decided, this should be raised with the implementation team (i.e. Patrick Johnson) for a decision. `olcoverrides.csv` should then be updated along with `econsult.csv` and/or `accurx.csv`.
	The goal from this step is get the "No OLC clashes detected" console output when the check is run. 

6. Run the following to update the set of eConsult yaml files

	```bash
	python sjr-config/utils/rulecreation/processolc.py
	```

7. Run the following to update the accuRx yaml file

	```bash
	python sjr-config/utils/rulecreation/processaccurx.py
	```
### PKB update
1. The PKB updates are posted in the #sjr-third-party-updates Slack channel, usually by Matt Deaves.
2. Update `sjr-config/configurations/Journeys/patientsKnowBest/*.yaml` with the practice changes - the files are usually at CCG level.

### Engage update

1. The Engage updates are raised through Service Now service requests (search for "NHS App Supplier Standard Change Request"). A member of the T2 support team should then post the file in the sjr-third-party-updates Slack channel. They are provided as an xlsx file, with additions and deletions on separate worksheets. Contact a member of the T3 support team to forward the file if you do not have access to Service Now.

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

3. The CI pipeline should run automatically when you create the new tag. If for some reason this does not complete successfully you can run the nhsapp CI pipeline [https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=4](https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=4) with the following options for a minimal build:

	*Branch/tag* set to your new tag of `nhsapp`.
	
	*Stages to run* with Integration Tests and Generate Code Coverage stages *unset*

4. Create a new Release from your Tag using the NHSApp Release Pipeline (Multicluster) in ADO [https://dev.azure.com/nhsapp/NHS%20App/_release?_a=releases&view=mine&definitionId=16](https://dev.azure.com/nhsapp/NHS%20App/_release?_a=releases&view=mine&definitionId=16) 

5. Progress the release through to staging, and give it a quick test there to ensure it looks ok. You may need to seek approval to deploy to staging. When you are happy, notify the relevant release channel that you are deploying to cold and get approval for that step (Production Deploy). Test in cold with a live account [https://www-cold.production.nhsapp.service.nhs.uk/](https://www-cold.production.nhsapp.service.nhs.uk/). When you are happy get approval for the Production Release step. If you deploy to cold and for some reason don't progress to switching live, let Ops know so they can clear the release down to save space.

6. Create the new file of eConsult live surgeries:

	```bash
	  awk -F, '{print $8","$7}' sjr-config/utils/rulecreation/econsult.csv > sjr-config/utils/rulecreation/200729-practices-live.csv
	```

	and add to the [eConsult SharePoint folder](https://hscic365.sharepoint.com/:f:/r/sites/eConsult/Shared%20Documents/eConsult%20Practice%20Management?csf=1&web=1&e=xfDYrk). Do this even if there were no changes to eConsult for the release. 
	
	Email the file to liz.jones@webgp.com and data-bi@econsult.health with subject 'NHS App config'.

7. Add the latest sjr-config/utils/rulecreation/engage.csv to the [Engage SharePoint folder](https://hscic365.sharepoint.com/sites/EngageHealthSystemsLtd/Shared%20Documents/Forms/AllItems.aspx?csf=1&web=1&e=097t7s&cid=1e0bd1d8%2D3964%2D4bba%2D80e2%2Ddc7b3b6abca1&RootFolder=%2Fsites%2FEngageHealthSystemsLtd%2FShared%20Documents%2FEngage%20Practice%20Management&FolderCTID=0x0120009F236CF489C42E4E8C5F68FA5FC260FE) and rename the csv file to the format `YYMMDD-practices-live.csv`. Do this even if there were no changes to Engage for the release.

8. If you updated a release branch then cherry pick the update back into `develop`. This will ensure the next release cut is up-to-date with SJR.
