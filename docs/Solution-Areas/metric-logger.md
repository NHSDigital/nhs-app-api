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

AppointmentBooking Body:
```bash
[
    {
        "Id": "77ae16c3-fbb7-4aa1-861b-86a5949488bf",
        "Timestamp": "2022-05-04T10:13:06.83016Z",
        "AuditId": "77ae16c3-fbb7-4aa1-861b-86a5949488bf",
        "NhsLoginSubject": "5abf0a97-2898-41a8-b04c-6f94d3b541eb",
        "NhsNumber": "969 223 7680",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "Appointments_Book_Response",
        "Details": "Appointment successfully booked for appointment with id: ddf1a37f-f46b-4147-8b1b-d5d402610358 and startDateTime: 2022-05-07T09:57:19.6877071+00:00",
        "ApiVersion": "v2.8.2",
        "WebVersion": "2.8.2 (commit:f57f22a81)",
        "NativeVersion": "ios 2.8.0",
        "Environment": "onboardingaos",
        "IntegrationReferrer": null,
        "SessionId": "c69711d8-2013-4dd1-acad-5bbbfecdb35e",
        "ProofLevel": "P9",
        "id": "76d83765-ec5a-475b-aa60-92cef2f899db",
        "_rid": "RuVQAKPVga8qDQYAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8qDQYAAAAAAA==/",
        "_etag": "\"c30075fe-0000-1100-0000-627251b20000\"",
        "_attachments": "attachments/",
        "_ts": 1651659186
    },
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

OrganDonationRegistration Body:
```bash
[
    {
        "Id": "af17de5d-79c8-43e9-a741-2ff5f54368ff",
        "Timestamp": "2022-03-02T15:33:51.3049067Z",
        "AuditId": "af17de5d-79c8-43e9-a741-2ff5f54368ff",
        "NhsLoginSubject": "cb09b2b9-aae3-4162-96d3-0eea6ae938d8",
        "NhsNumber": "969 211 3868",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Emis",
        "Operation": "OrganDonation_Registration_Response",
        "Details": "The organ donation decision has been successfully registered",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:22377e854)",
        "NativeVersion": "android 2.5.0",
        "Environment": "preview",
        "IntegrationReferrer": null,
        "id": "749cc2b8-c39c-44ac-b256-066372dd6e99",
        "_rid": "RuVQAKPVga+oDAEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga+oDAEAAAAAAA==/",
        "_etag": "\"d1014c1c-0000-1100-0000-621f8e5f0000\"",
        "_attachments": "attachments/",
        "_ts": 1646235231
    }
]
```

UpdateOrganDonation Body:
```bash
[
    {
        "Id": "97b8f02f-0682-4a82-a24f-71b16e141ba4",
        "Timestamp": "2022-03-01T16:26:32.2328691Z",
        "AuditId": "97b8f02f-0682-4a82-a24f-71b16e141ba4",
        "NhsLoginSubject": "2d97a264-996d-4294-9b61-3e87e3c1103f",
        "NhsNumber": "966 118 3279",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "OrganDonation_Update_Response",
        "Details": "The organ donation decision has been successfully updated",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:1615267fb)",
        "NativeVersion": "ios 2.4.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "id": "e67ddb18-03ad-42c6-90c2-9e5e21c16b0c",
        "_rid": "RuVQAKPVga-G9QAAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga-G9QAAAAAAAA==/",
        "_etag": "\"c801b5f8-0000-1100-0000-621e49380000\"",
        "_attachments": "attachments/",
        "_ts": 1646151992
    }
]
```

GetOrganDonation Body:
```bash
[
    {
        "Id": "a10231b7-54e3-4c04-b17f-f2dc022afd98",
        "Timestamp": "2022-02-17T18:47:56.6791548Z",
        "AuditId": "a10231b7-54e3-4c04-b17f-f2dc022afd98",
        "NhsLoginSubject": "e85fb7cc-2bcf-4f26-b3f0-6b27524c3692",
        "NhsNumber": "969 211 3612",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Emis",
        "Operation": "OrganDonation_Get_Response",
        "Details": "A default organ donation registration has been generated",
        "ApiVersion": "b90ac9cb548192c13e127ec8e08e4ea6e94f42d9",
        "WebVersion": "b90ac9cb548192c13e127ec8e08e4ea6e94f42d9 (commit:b90ac9cb5)",
        "NativeVersion": "ios 2.5.0",
        "Environment": "scratch12",
        "id": "eb05879d-8531-4353-a287-15a7410329e5",
        "_rid": "RuVQAKPVga+dAwAAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga+dAwAAAAAAAA==/",
        "_etag": "\"7200b265-0000-1100-0000-620e985c0000\"",
        "_attachments": "attachments/",
        "_ts": 1645123676
    }
]
```

RepeatPrescription Body:
```bash
[
    {
        "Id": "79737fd7-e48d-49de-a237-fd7a806877bf",
        "Timestamp": "2022-04-23T23:53:33.0602823Z",
        "AuditId": "79737fd7-e48d-49de-a237-fd7a806877bf",
        "NhsLoginSubject": "ec66fe04-e8e3-4401-aa41-0b99c69c5c57",
        "NhsNumber": "969 223 8318",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "RepeatPrescriptions_OrderRepeatMedications_Response",
        "Details": "Repeat prescription request successfully created with course ids: FakeCourse1",
        "ApiVersion": "v2.8.1",
        "WebVersion": "2.8.1 (commit:15543227b)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "c23944a4-baf4-4db4-997e-aaa9a4de5311",
        "ProofLevel": "P9",
        "id": "1ce6bd64-a3e6-48e4-aa3f-f2bf5e41fda6",
        "_rid": "RuVQAKPVga9bdAUAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga9bdAUAAAAAAA==/",
        "_etag": "\"88019389-0000-1100-0000-6264917d0000\"",
        "_attachments": "attachments/",
        "_ts": 1650758013
    }
]
```

OrganDonationWithdraw Body:
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
        "Operation": "OrganDonation_Withdraw_Response",
        "Details": "The organ donation decision has been successfully Withdrawn",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:7d710d306)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "ProofLevel": null,
        "id": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "_rid": "RuVQAKPVga8bowEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8bowEAAAAAAA==/",
        "_etag": "\"e4011bab-0000-1100-0000-622890e20000\"",
        "_attachments": "attachments/",
        "_ts": 1646825698
    }
]
```

BiometricsToggle Body:
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
        "Operation": "BiometricsRegistration_Decision",
        "Details": "Biometrics toggled. optIn=true",
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

CreateNominatedPhramacy Body:
```bash
[
    {
        "Id": "7f3e2d28-0f70-44f9-8f8e-0ac38ee1a23e",
        "Timestamp": "2022-02-18T12:44:37.280269Z",
        "AuditId": "7f3e2d28-0f70-44f9-8f8e-0ac38ee1a23e",
        "NhsLoginSubject": "81670141-803b-4aa4-a9a5-4a8e7a8ed245",
        "NhsNumber": "966 103 3625",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Disconnected",
        "Operation": "NominatedPharmacy_Update_Response",
        "Details": "Successfully created new nominated pharmacy registration to Z0000",
        "ApiVersion": "f3242696750c493cae5c9eacc4597cc749653179",
        "WebVersion": "f3242696750c493cae5c9eacc4597cc749653179 (commit:f32426967)",
        "NativeVersion": null,
        "Environment": "scratch12",
        "SessionId": "af664631-15f7-4205-8ebb-f9e9bc45abc2",
        "id": "434a7124-0a42-497f-99f8-fa69ceea4a3f",
        "_rid": "RuVQAKPVga8WBwAAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8WBwAAAAAAAA==/",
        "_etag": "\"9f00cb2a-0000-1100-0000-620f94b50000\"",
        "_attachments": "attachments/",
        "_ts": 1645188277
    }
]
```

UpdateNominatedPharmacy Body:
```bash
[
    {
        "Id": "7f3e2d28-0f70-44f9-8f8e-0ac38ee1a23e",
        "Timestamp": "2022-02-18T12:44:37.280269Z",
        "AuditId": "7f3e2d28-0f70-44f9-8f8e-0ac38ee1a23e",
        "NhsLoginSubject": "81670141-803b-4aa4-a9a5-4a8e7a8ed245",
        "NhsNumber": "966 103 3625",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Disconnected",
        "Operation": "NominatedPharmacy_Update_Response",
        "Details": "Successfully updated nominated pharmacy from Z0000 to Z0001",
        "ApiVersion": "f3242696750c493cae5c9eacc4597cc749653179",
        "WebVersion": "f3242696750c493cae5c9eacc4597cc749653179 (commit:f32426967)",
        "NativeVersion": null,
        "Environment": "scratch12",
        "SessionId": "af664631-15f7-4205-8ebb-f9e9bc45abc2",
        "id": "434a7124-0a42-497f-99f8-fa69ceea4a3f",
        "_rid": "RuVQAKPVga8WBwAAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8WBwAAAAAAAA==/",
        "_etag": "\"9f00cb2a-0000-1100-0000-620f94b50000\"",
        "_attachments": "attachments/",
        "_ts": 1645188277
    }
]
```

SilverIntegrationJumpOff Body:
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
        "Operation": "SilverIntegration_JumpOff_Click",
        "Details": "The user has jumped off to an integration partner",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:7d710d306)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "ProofLevel": "P9",
        "ProviderId":"ers",
        "ProviderName":"Electronic Referral Service",
        "JumpOffId":"manageYourReferral",
        "id": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "_rid": "RuVQAKPVga8bowEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8bowEAAAAAAA==/",
        "_etag": "\"e4011bab-0000-1100-0000-622890e20000\"",
        "_attachments": "attachments/",
        "_ts": 1646825698
    },
]
```

GoldIntegrationJumpOff Body:
```bash
[
    {
        "Id": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "Timestamp": "2022-03-09T11:34:58.2045278Z",
        "AuditId": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "NhsLoginSubject": "24fe1e19-df2c-423e-961a-7c717c0b1333",
        "NhsNumber": "966 118 3252",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "eConsult",
        "Operation": "GoldIntegration_JumpOff_Click",
        "Details": "The user has jumped off to an integration partner",
        "ApiVersion": "develop",
        "WebVersion": "develop (commit:7d710d306)",
        "NativeVersion": "android 2.5.0",
        "Environment": "onboardingsandpit",
        "IntegrationReferrer": null,
        "SessionId": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "ProofLevel": "P9",
        "ProviderId":"eConsult",
        "ProviderName":"eConsult",
        "JumpOffId":"onlineConsultation",
        "id": "54842c1d-5903-49f9-b5a9-bd9c9caf6f21",
        "_rid": "RuVQAKPVga8bowEAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8bowEAAAAAAA==/",
        "_etag": "\"e4011bab-0000-1100-0000-622890e20000\"",
        "_attachments": "attachments/",
        "_ts": 1646825698
    }
]
```

PatientRecordSectionView Body:
```bash
[
    {
        "Id": "213d7ae1-1e26-4b98-a4ef-75994fd0846b",
        "Timestamp": "2022-05-27T11:12:47.3068548Z",
        "AuditId": "213d7ae1-1e26-4b98-a4ef-75994fd0846b",
        "NhsLoginSubject": "AFF0A112-X001-000F-BF99-147D64025:S0",
        "NhsNumber": "550 019 0757",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Vision",
        "Operation": "PatientRecord_Section_View_Response",
        "Details": "Patient record TEST RESULTS successfully retrieved.",
        "ApiVersion": "develop",
        "WebVersion": "develop",
        "NativeVersion": null,
        "Environment": "loadtest",
        "SessionId": "c69711d8-2013-4dd1-acad-5bbbfecdb35e",
        "ProofLevel": "P9",
        "id": "95ad044c-3f9a-42c4-8d13-e8754b9bee63",
        "_rid": "XR9GALdybyjcGFMCAADAAw==",
        "_self": "dbs/XR9GAA==/colls/XR9GALdybyg=/docs/XR9GALdybyjcGFMCAADAAw==/",
        "_etag": "\"91002cc5-0000-1100-0000-6290b22f0000\"",
        "_attachments": "attachments/",
        "_ts": 1653649967
    }
]
```

Login_Device Body:
```bash
[
    {
        "Id": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "Timestamp": "2022-05-11 07:59:59.957+00",
        "AuditId": "8f276db5-db9d-4d68-a6ba-760f07f4adb8",
        "NhsLoginSubject": "24fe1e19-df2c-423e-961a-7c717c0b1333",
        "NhsNumber": "966 118 3252",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Fake",
        "Operation": "Login_Device",
        "Details": "Device details returned: nhsapp-android/2.7.0 nhsapp-manufacturer/samsung nhsapp-model/SM-G998B nhsapp-os/12 nhsapp-architecture/arm64",
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

LastLoginPatientIdentifier Body:
```bash
[
    {
        "Id": "dbd2221d-3327-4ec4-adc2-c647bd9604da",
        "Timestamp": "2022-02-15T16:16:38.8840116Z",
        "AuditId": "dbd2221d-3327-4ec4-adc2-c647bd9604da",
        "NhsLoginSubject": "3c90d2d7-5474-4acd-8722-c7c332881421",
        "NhsNumber": "969 211 3736",
        "IsActingOnBehalfOfAnother": false,
        "Supplier": "Unknown",
        "Operation": "CitizenId_Session_Create_Request",
        "Details": "Create Citizen Id Session",
        "ApiVersion": null,
        "WebVersion": null,
        "NativeVersion": null,
        "id": "da3fd40f-1761-46d6-b5fb-fc72b9a0e269",
        "_rid": "RuVQAKPVga8BAAAAAAAAAA==",
        "_self": "dbs/RuVQAA==/colls/RuVQAKPVga8=/docs/RuVQAKPVga8BAAAAAAAAAA==/",
        "_etag": "\"0102c1af-0000-1100-0000-620bd5a30000\"",
        "_attachments": "attachments/",
        "_ts": 1644942755
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