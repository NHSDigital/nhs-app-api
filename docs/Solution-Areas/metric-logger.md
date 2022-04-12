# Metric Logger
The Metric Logger is a Function App built using .NET 6
It is based on the NHS Analytics App. It's purpose is to provide a more reliable method of ingesting App events such as Logins and Consents by pulling these events from the Audit Event Hub as opposed to Splunk Cloud

## Running the Metric Logger Locally 

You can start the environment needed for the Metric Logger Function App by running this command in the root of the metric logger function app folder

```bash
make run
```
Then it is just a case of running the function app locally using your IDE
## Sending a Metric Event to your local instance of the Metric Logger

You can send a Metric Event to the local instance of the Metric Logger App you are running by using the following details in a POST request in Postman

URL:
```bash
http://localhost:7071/api/AuditLog_Etl_Http
```

Login Body:
```bash
[
    {
        "AuditId": "3440f219-cb5a-4c8b-a3f8-17fc59280b7c",
        "NhsLoginSubject": "cb09b2b9-aae3-4162-96d3-0eea6ae938d8",
        "NhsNumber": "969 211 3868",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Unknown",
        "Operation": "Login_Success",
        "Details": "Successful Login with",
        "ApiVersion": "de565225610ed8e3cf3caab6024b61184f8e28a9",
        "WebVersion": "de565225610ed8e3cf3caab6024b61184f8e28a9 (commit:de5652256)",
        "NativeVersion": null,
        "Environment": "scratch12",
        "SessionId": "test-session",
        "Timestamp": "2022-02-21T16:29:58.8572498Z",
        "ProofLevel": "P5",
        "ODS": "",
        "Referrer": "This is working",
        "IntegrationReferrer": "IntRef1"
    }
]
```

Consent Body:
```bash
[
    {
        "AuditId": "3440f819-cb5a-4c8b-a3f8-17fc59280b7c",
        "NhsLoginSubject": "cb09b2b9-aae3-4162-96d3-0eea6ae938d8",
        "NhsNumber": "969 211 3868",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Unknown",
        "Operation": "TermsAndConditions_RecordConsent_Response",
        "Details": "Initial Consent Successfully recorded",
        "ApiVersion": "de565225610ed8e3cf3caab6024b61184f8e28a9",
        "WebVersion": "de565225610ed8e3cf3caab6024b61184f8e28a9 (commit:de5652256)",
        "NativeVersion": null,
        "Environment": "scratch12",
        "SessionId": "test-session",
        "Timestamp": "2022-02-21T17:29:58.8572498Z",
        "ProofLevel": "P5",
        "ODS": "",
        "Referrer": "This is working",
        "IntegrationReferrer": "IntRef1"
    }
]
```
## Running the Metric Logger Integration Tests Locally 

The ideal way of checking that changes you have made locally work is by kicking off the Integration test environment which can be done by running this command in the root of the metric logger function app folder

```bash
make run-inttests
```

Then the tests in the IntegrationTests project folder can be run from the IDE and you can connect to the local postgres instance to verfify the data being sent to the local instance of the Analytics DB

## Source Code 

The sql scripts contained within the **metriclogfunctionapp/Database** folder are solely used for the purpose of spinning up a local instance of the Analytics database required when executing the integration tests.

These scripts are duplicates and where possible will be kept in line with those contained in the nhsapp-analytics repo.
## Build Pipeline 

A build pipeline named **nhsapp-metriclogfunctionapp** exists in Azure DevOps under ***Pipelines/Pipelines***. This is triggered on changes to the source code and will run unit & integration tests as well as compiling the application and publishing the result.
## Release Pipeline

A release pipeline named ***MetricLog Event Consumer App*** exists in Azure DevOps under ***Pipelines/Releases***. This is used to release the function app across different environments (Sandbox, Dev, Staging, Production).