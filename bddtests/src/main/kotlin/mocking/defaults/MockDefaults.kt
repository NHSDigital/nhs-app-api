package mocking.defaults

import mocking.MockingClient
import mocking.emis.EmisConfiguration
import config.Config
import mocking.MockingConfiguration
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.tpp.models.*
import mocking.vision.VisionConstants
import mocking.vision.models.*
import models.Patient
import worker.models.demographics.TppUserSession
import worker.models.session.UserSessionRequest

class MockDefaults(val config: Config, val mockingClient: MockingClient = MockingClient.instance) {

    companion object {
        const val DEFAULT_END_USER_SESSION_ID: String = "7YjG1LYkOkSY1iAcXGG8ZU"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"
        const val DEFAULT_ODS_CODE: String = "A29928"
        const val DEFAULT_ODS_CODE_TPP: String = "KGPD"
        const val DEFAULT_ODS_CODE_VISION: String = "X00100"
        const val DEFAULT_SESSION_ID: String = "AJYF0ufQI6tTpdfwaXAt"
        const val DEFAULT_ACCESS_TOKEN: String = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3WGlNdHVGSHN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa0thMkZZZHA0In0.eyJqdGkiOiIwNjBlZjM4Yy00YmRlLTQ1ZjgtYjExMy1iOGYzZjVjYWJjOGQiLCJleHAiOjE1MjM5NTg5MTYsIm5iZiI6MCwiaWF0IjoxNTIzOTU4NjE2LCJpc3MiOiJodHRwczovL2tleWNsb2FrLmRldjEuc2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9OSFMiLCJhdWQiOiJuaHMtb25saW5lLXBvYyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZDIyLTlkNGY2YmYwZjUzMSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9jIiwiYXV0aF90aW1lIjoxNTIzOTU4NTkyLCJzZXNzaW9uX3N0YXRlIjoiYTRmYmYwN2EtNGM3MS00MTdjLWE2OTYtYmUxNjQ3MDIwOGM0IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W10sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hbmFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSIsInZpZXctaWRlbnRpdHktcHJvdmlkZXJzIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsImltcGVyc29uYXRpb24iLCJyZWFsbS1hZG1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNlcnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3LWF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIiwicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlbnRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHMiLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbnRzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5hZ2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6IlJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF91c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsImZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJlYWxtYWRtaW5AZ21haWwuY29tIn0.D2nSVJbZ7M2JZosiC6z-HXx7-Rg1n7w7CCKvWtBzErJVDIedvS5y6syxQnJbtl0yITYM4qP-gN0Ji13qnwu0wjy-NorXvG7BOB5wl2SXekaaphXjv9e6NshQ5SEhyV1hMzfPRqLkZbpETjEOdPiMziG6k8sZCpast3c3diKb96dxjVIOhPayf2P9Z75b-qnegFuV1LkD9mIkGDyA7t5givfouskPSr09EKyxHf_m7kjPipy39cKODgcbsyYpwqAmHYaHJGsqIZYDPTCjvzmkrZOQlGJ_sXAVmxrZY8psUZ7MKeFd4l9xwvfi4N-3FFT5D4_tJq0Yp3RW5Bs3JVc1ig"
        const val DEFAULT_BEARER_TOKEN: String = "Bearer $DEFAULT_ACCESS_TOKEN"

        const val TPP_API_VERSION = "1"
        const val DEFAULT_TPP_UUID = "af0a8175-e6c2-4c49-883e-020b2b3600f9"
        const val DEFAULT_TPP_PROVIDER_ID = "b891fc3b51d5e7c1"
        const val DEFAULT_TPP_SESSION_ID = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"
        val DEFAULT_TPP_APPLICATION = Application(
                name = "NhsApp",
                version = "1.0",
                providerId = DEFAULT_TPP_PROVIDER_ID,
                deviceType = "NhsApp"
        )

        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration("D66BA979-60D2-49AA-BE82-AEC06356E41F", "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }

        val patient = Patient.getDefault("EMIS")
        val patientTpp = Patient.getDefault("TPP")
        val patientVision = Patient.getDefault("VISION")

        val tppAuthenticateRequest = Authenticate(
                apiVersion = TPP_API_VERSION,
                accountId = patientTpp.accountId,
                passphrase = patientTpp.passphrase,
                unitId = DEFAULT_ODS_CODE_TPP,
                uuid = DEFAULT_TPP_UUID,
                application = DEFAULT_TPP_APPLICATION
        )

        val tppAuthenticateReplyResponse = AuthenticateReply(
                patientId = patientTpp.patientId,
                onlineUserId = patientTpp.onlineUserId,
                uuid = "af0a8175-e6c2-4c49-883e-020b2b3600f9",
                user = User(
                        person = Person(
                                patientId = patientTpp.patientId,
                                dateOfBirth = patientTpp.dateOfBirth,
                                gender = patientTpp.sex.name,
                                nationalId = NationalId(
                                        type = "NHS",
                                        value = patientTpp.nhsNumbers.first()
                                ),
                                personName = PersonName(
                                        name = "${patientTpp.title} ${patientTpp.firstName} ${patientTpp.surname}"
                                )
                        )
                )
        )

        val tppNonExistingAccountIdErrorResponse = Error(
                errorCode = "9",
                userFriendlyMessage = "There was a problem logging on",
                uuid = "47788ae4-10e9-4f2c-9043-e08d285b67b6"
        )

        val visionUserSession = VisionUserSession(
                Patient.aderynCanon.rosuAccountId,
                Patient.aderynCanon.apiKey,
                Patient.aderynCanon.odsCode)

        val visionGetConfiguration = ServiceDefinition(
                VisionConstants.configurationName,
                VisionConstants.configurationVersion)

        val visionConfigurationResponse = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(PatientNumber(number = patientVision.nhsNumbers[0])),
                name = getFullPatientName(patientVision)))

        fun getFullPatientName(patient: Patient): String{
          return "${patient.title} ${patient.firstName} ${patient.surname}"
        }

    }
}