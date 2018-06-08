package mocking.defaults
import mocking.MockingClient
import mocking.emis.EmisConfiguration
import config.Config
import mocking.MockingConfiguration
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import models.Patient
import worker.models.session.UserSessionRequest

class MockDefaults(val config: Config, val mockingClient: MockingClient = MockingClient.instance) {

    fun mock() {
        mockingClient.clearWiremock()
        mockingClient.favicon()

        mockingClient.forCitizenId {
            initialLoginRequest(config.cidRedirectUri, config.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            createAccountRequest(config.cidRedirectUri, config.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest()
                    .respondWithRedirectResponse(userSessionRequest.authCode!!)
        }

        mockingClient.forCitizenId {
            tokenRequest(userSessionRequest.codeVerifier, userSessionRequest.authCode)
                    .respondWithSuccess(
                            "access_token",
                            "30",
                            "30",
                            "refresh_token",
                            "token_type")
        }
        mockingClient.forCitizenId {
            userInfoRequest("Bearer access_token")
                    .respondWithSuccess()
        }

        mockingClient.forEmis {
            endUserSessionRequest()
                    .respondWithSuccess(DEFAULT_END_USER_SESSION_ID)
        }

        mockingClient.forEmis {
            sessionRequest(patient)
                    .respondWithSuccess(patient, AssociationType.Self)
        }

        val appointmentSlotsMetadataResponse = GetAppointmentSlotsMetaResponseModel(
                locations = arrayListOf(
                        Location(
                                locationId = 1,
                                locationName = "Surgery",
                                numberAndStreet = "12 Old Road",
                                town = "Healthton",
                                postcode = "HE4 1TH"
                        )
                ),
                sessions = arrayListOf(
                        Session(
                                sessionId = 1,
                                sessionName = "Foot clinic",
                                locationId = 1,
                                defaultDuration = 60,
                                sessionType = SessionType.Timed,
                                numberOfSlots = 2
                        )
                ),
                sessionHolders = arrayListOf(
                        SessionHolder(
                                clinicianId = 1,
                                displayName = "Miss Sally",
                                forenames = "Sally",
                                surname = "Dobson",
                                title = "Ms",
                                sex = Sex.Female
                        )
                )
        )

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(appointmentSlotsMetadataResponse)
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWithSuccess(
                                    sessionDate = "2017-05-08",
                                    sessionId = 2,
                                    slots = arrayListOf(
                                            AppointmentSlot(
                                                    slotId = 1,
                                                    startTime = "2017-05-08T13:00:00.000Z",
                                                    endTime = "2017-05-08T13:15:00.000Z",
                                                    slotTypeName = "GP appointment",
                                                    slotTypeStatus = SlotTypeStatus.Practice
                                            ),
                                            AppointmentSlot(
                                                    slotId = 2,
                                                    startTime = "2017-05-08T13:00:00.000Z",
                                                    endTime = "2017-05-08T13:15:00.000Z",
                                                    slotTypeName = "Hearing test",
                                                    slotTypeStatus = SlotTypeStatus.Practice
                                            )
                                    )
                            )

                }
    }

    companion object {
        const val DEFAULT_END_USER_SESSION_ID: String = "7YjG1LYkOkSY1iAcXGG8ZU"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"
        const val DEFAULT_ODS_CODE: String = "A29928"
        const val DEFAULT_SESSION_ID: String = "AJYF0ufQI6tTpdfwaXAt"
        const val DEFAULT_ACCESS_TOKEN: String = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3WGlNdHVGSHN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa0thMkZZZHA0In0.eyJqdGkiOiIwNjBlZjM4Yy00YmRlLTQ1ZjgtYjExMy1iOGYzZjVjYWJjOGQiLCJleHAiOjE1MjM5NTg5MTYsIm5iZiI6MCwiaWF0IjoxNTIzOTU4NjE2LCJpc3MiOiJodHRwczovL2tleWNsb2FrLmRldjEuc2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9OSFMiLCJhdWQiOiJuaHMtb25saW5lLXBvYyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZDIyLTlkNGY2YmYwZjUzMSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9jIiwiYXV0aF90aW1lIjoxNTIzOTU4NTkyLCJzZXNzaW9uX3N0YXRlIjoiYTRmYmYwN2EtNGM3MS00MTdjLWE2OTYtYmUxNjQ3MDIwOGM0IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W10sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hbmFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSIsInZpZXctaWRlbnRpdHktcHJvdmlkZXJzIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsImltcGVyc29uYXRpb24iLCJyZWFsbS1hZG1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNlcnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3LWF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIiwicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlbnRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHMiLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbnRzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5hZ2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6IlJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF91c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsImZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJlYWxtYWRtaW5AZ21haWwuY29tIn0.D2nSVJbZ7M2JZosiC6z-HXx7-Rg1n7w7CCKvWtBzErJVDIedvS5y6syxQnJbtl0yITYM4qP-gN0Ji13qnwu0wjy-NorXvG7BOB5wl2SXekaaphXjv9e6NshQ5SEhyV1hMzfPRqLkZbpETjEOdPiMziG6k8sZCpast3c3diKb96dxjVIOhPayf2P9Z75b-qnegFuV1LkD9mIkGDyA7t5givfouskPSr09EKyxHf_m7kjPipy39cKODgcbsyYpwqAmHYaHJGsqIZYDPTCjvzmkrZOQlGJ_sXAVmxrZY8psUZ7MKeFd4l9xwvfi4N-3FFT5D4_tJq0Yp3RW5Bs3JVc1ig"
        const val DEFAULT_BEARER_TOKEN: String = "Bearer $DEFAULT_ACCESS_TOKEN"

        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration("D66BA979-60D2-49AA-BE82-AEC06356E41F", "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }

        val patient = Patient.montelFrye

        val userSessionRequest = UserSessionRequest(
                authCode = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPSETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-7657b5e9e403.8d4c0a21-6483-4a52-9d47-6bcd737c634e",
                codeVerifier = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU"
        )
    }
}