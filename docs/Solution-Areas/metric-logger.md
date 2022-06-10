# Metric Logger
The Metric Logger is a Function App built using .NET 6
It is based on the NHS Analytics App. It's purpose is to provide a more reliable method of ingesting App events such as Logins and Consents by pulling these events from the Audit Event Hub as opposed to Splunk Cloud

## Running the Metric Logger Locally 

You can start the environment needed for the Metric Logger Function App by running this command in the root of the metric logger function app folder (this will spin up a database named analyticsdb)

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

SecondaryCareSummary Body:
```bash
[
    {
        "Id": "db9da242-3572-4a7d-b610-78def49488c6",
        "Timestamp": "2022-03-28T09:12:54.7260781Z",
        "AuditId": "db9da242-3572-4a7d-b610-78def49488c6",
        "NhsLoginSubject": "7fa5d376-7b2f-42e6-9f27-793f38be0355",
        "NhsNumber": "111 111 1111",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Disconnected",
        "Operation": "SecondaryCare_GetSummary_Response",
        "Details": "Secondary Care Summary successfully retrieved. Total Referrals: 3, Total Upcoming Appointments: 3",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:20ea9e3a4)",
        "NativeVersion": "ios 2.6.0",
        "Environment": "scratch19",
        "IntegrationReferrer": null,
        "SessionId": "3024ba94-9614-44dd-bea8-d88303bd9282",
        "ProofLevel": "P9",
        "id": "d9dd8af3-0799-43ec-b7f3-6312617fc7c3",
        "_rid": "RuVQAKPVga8POQMAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8POQMAAAAAAA==/",
        "_etag": "\"50015471-0000-1100-0000-62417c160000\"",
        "_attachments": "attachments/",
        "_ts": 1648458774
    }
]
```

MedicalRecordView Body:
```bash
[
    {
        "Id": "d98018ae-a8e7-490f-a3dd-9686114ac6c9",
        "Timestamp": "2022-05-05T07:58:30.0660938Z",
        "AuditId": "d98018ae-a8e7-490f-a3dd-9686114ac6c9",
        "NhsLoginSubject": "1b09a0ba-c36b-403f-bca5-7cfd7cf2cb95",
        "NhsNumber": "969 223 7672",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "PatientRecord_View_Response",
        "Details": "Patient record successfully retrieved. hasSummaryRecordAccess=False, hasDetailedRecordAccess=False",
        "ApiVersion": "v2.8.2",
        "WebVersion": "2.8.2 (commit:f57f22a81)",
        "NativeVersion": null,
        "Environment": "onboardingaos",
        "IntegrationReferrer": null,
        "SessionId": "d6a628c7-1589-41c3-8861-dbc238db7d02",
        "ProofLevel": "P9",
        "id": "5928070c-ef35-42ff-ac9d-7ffb0bcc0681",
        "_rid": "RuVQAKPVga9EJAYAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga9EJAYAAAAAAA==/",
        "_etag": "\"e9009237-0000-1100-0000-627383a60000\"",
        "_attachments": "attachments/",
        "_ts": 1651737510
    }
]
```

NotificationToggle Body:
```bash
[
    {
        "Id": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "Timestamp": "2022-03-09T11:34:58.2045278Z",
        "AuditId": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "NhsLoginSubject": "24fe1e19-df2c-423e-961a-7c717c0b1333",
        "NhsNumber": "966 118 3252",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "NotificationToggle_Response",
        "Details": "Notification toggled. optIn=true",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:7d710d306)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "ProofLevel": "P9",
        "id": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "_rid": "RuVQAKPVga8bowEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8bowEAAAAAAA==/",
        "_etag": "\"e4011bab-0000-1100-0000-622890e20000\"",
        "_attachments": "attachments/",
        "_ts": 1646825698
    }
]
```

InitialPrompt body:
```bash
[
    {
        "Id": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "Timestamp": "2022-03-09T11:34:58.2045278Z",
        "AuditId": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "NhsLoginSubject": "24fe1e19-df2c-423e-961a-7c717c0b1333",
        "NhsNumber": "966 118 3252",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "InitialNotificationPrompt_Decision",
        "Details": "Initial notification prompt decision made. optIn=true",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:7d710d306)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "ProofLevel": "P9",
        "id": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "_rid": "RuVQAKPVga8bowEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8bowEAAAAAAA==/",
        "_etag": "\"e4011bab-0000-1100-0000-622890e20000\"",
        "_attachments": "attachments/",
        "_ts": 1646825698
    }
]
```

AppointmentCancellation Body:
```bash
[
    {
        "Id": "dffeb9a8-fd50-439c-b448-f6da59837821",
        "Timestamp": "2022-04-26T16:15:17.5871586Z",
        "AuditId": "dffeb9a8-fd50-439c-b448-f6da59837821",
        "NhsLoginSubject": "7e009dfe-4ece-4aec-abf9-065a6a14e3da",
        "NhsNumber": "966 103 3552",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Emis",
        "Operation": "Appointments_Cancel_Response",
        "Details": "Appointment successfully cancelled for appointment with id: 237710",
        "ApiVersion": "pr7858",
        "WebVersion": "pr7858 (commit:c1185c1f9)",
        "NativeVersion": null,
        "Environment": "scratch3",
        "IntegrationReferrer": null,
        "SessionId": "6f3aa88b-d544-4ca6-bd06-002e8e555fdf",
        "ProofLevel": "P9",
        "id": "febec662-86e5-40af-8513-a21c90116bf5",
        "_rid": "RuVQAKPVga-6sQUAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga-6sQUAAAAAAA==/",
        "_etag": "\"0a02c6f8-0000-1100-0000-62681a950000\"",
        "_attachments": "attachments/",
        "_ts": 1650989717
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