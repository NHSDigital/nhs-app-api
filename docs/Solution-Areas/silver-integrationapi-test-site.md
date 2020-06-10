# Silver integration API test site.
The purpose of these pages is to simulate a third party provider site and test the functionality of the Silver Intergration JS Api.

## Hosting
These pages are currently hosting on a storage account in the Non-live subscription to serve a basic static website - https://silverintegrationtestsa.z33.web.core.windows.net/

Currently the html is referring to a js file hosted along side, but will be changed to point at a scratch environment when it is deployed. To update the site, you will need access to the silverintegrationtestsa storage account.

## Updating the pages
To update the site, you will need access to the `silverintegrationtestsa` storage account.

One method to update the site is to download and install [Azure Storage Explorer](https://azure.microsoft.com/en-gb/features/storage-explorer/), and connect to the `silverintegrationtestsa` storage account.

Once connected, navigate to `Local & Attached > Storage Accounts > silverintegrationtestsa > Blob Containers > $web` and you can drag-and-drop to replace the files

## Running locally
In order to run the silver integration site locally and use it in the app you will need to add a host file entry for `silver.local.bitraft.io`.

When running locally a nginx docker image is used with a bind mount to expose the local static website and allow you to live edit the site.

This also allows the local site to use the static `nhsapp.js` file from your locally running web application.
