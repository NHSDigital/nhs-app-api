package mocking

import mocking.emis.EmisConfiguration
import config.Config
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.models.*
import mocking.emis.models.appointmentslots.MetaSession
import models.Patient
import worker.models.session.UserSessionRequest

const val DEFAULT_END_USER_SESSION_ID: String = "Ab42ZoP21dT4JE12avEWQ5"

class MockDefaults(val config: Config, val mockingClient: MockingClient = MockingClient.instance) {

    fun mock() {
        mockingClient.resetWiremock()
        mockingClient.favicon()

        mockingClient.forCitizenId {
            initialLoginRequest(config.cidRedirectUri, config.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest()
                    .respondWithRedirectResponse()
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
            userInfoRequest("our_bearer_token")
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

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(GetAppointmentSlotsMetaResponseModel(
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
                                            MetaSession(
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
                            ))
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
                                            ), AppointmentSlot(
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
        @JvmStatic
        fun main(arguments: Array<String>) {
            val config = Config.instance
            MockDefaults(config).mock()
        }

        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration("D66BA979-60D2-49AA-BE82-AEC06356E41F", "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }

        val patient = Patient(
                title = "Mr",
                firstName = "John",
                surname = "Smith",
                odsCode = "A29928",
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                sessionId = "MT4vWCxTKXRYr7fFJWM3wB")

        val userSessionRequest = UserSessionRequest(
                "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
                "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPSETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-7657b5e9e403.8d4c0a21-6483-4a52-9d47-6bcd737c634e"
        )
    }
}