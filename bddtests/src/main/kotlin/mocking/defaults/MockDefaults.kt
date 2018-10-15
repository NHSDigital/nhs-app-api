package mocking.defaults

import mocking.MockingClient
import mocking.emis.EmisConfiguration
import config.Config
import constants.TppConstants
import mocking.MockingConfiguration
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.User
import mocking.tpp.models.Person
import mocking.tpp.models.NationalId
import mocking.tpp.models.PersonName
import mocking.tpp.models.Error
import mocking.vision.Demographics.Demographics
import mocking.vision.Demographics.Name
import mocking.vision.Demographics.PrimaryAddress
import mocking.vision.VisionConstants
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession

import models.Patient

class MockDefaults(val config: Config, val mockingClient: MockingClient = MockingClient.instance) {

    companion object {
        const val DEFAULT_END_USER_SESSION_ID: String = "7YjG1LYkOkSY1iAcXGG8ZU"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"
        const val DEFAULT_ODS_CODE: String = "A29928"
        const val DEFAULT_ODS_CODE_TPP: String = "KGPD"
        const val DEFAULT_ODS_CODE_VISION: String = "X00100"
        const val DEFAULT_SESSION_ID: String = "AJYF0ufQI6tTpdfwaXAt"
        const val DEFAULT_ACCESS_TOKEN: String = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSl" +
                                                 "dUIiwia2lkIiA6ICI3WGlNdHVGS" +
                                                 "HN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa" +
                                                 "0thMkZZZHA0In0.eyJqdGkiOiIwNjB" +
                                                 "lZjM4Yy00YmRlLTQ1ZjgtYjExMy1iOGYzZjVjY" +
                                                 "WJjOGQiLCJleHAiOjE1MjM5NTg5MTYsI" +
                                                 "m5iZiI6MCwiaWF0IjoxNTIzOTU4NjE2LCJpc3Mi" +
                                                 "OiJodHRwczovL2tleWNsb2FrLmRldjEu" +
                                                 "c2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9O" +
                                                 "SFMiLCJhdWQiOiJuaHMtb25saW5lLXBv" +
                                                 "YyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZD" +
                                                 "IyLTlkNGY2YmYwZjUzMSIsInR5cCI6" +
                                                 "IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9j" +
                                                 "IiwiYXV0aF90aW1lIjoxNTIzOTU4NTkyL" +
                                                 "CJzZXNzaW9uX3N0YXRlIjoiYTRmYmYwN2EtNGM3M" +
                                                 "S00MTdjLWE2OTYtYmUxNjQ3MDIwOGM0" +
                                                 "IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W1" +
                                                 "0sInJlYWxtX2FjY2VzcyI6eyJyb2xl" +
                                                 "cyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb" +
                                                 "3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hb" +
                                                 "mFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSI" +
                                                 "sInZpZXctaWRlbnRpdHktcHJvdmlkZXJ" +
                                                 "zIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsI" +
                                                 "mltcGVyc29uYXRpb24iLCJyZWFsbS1hZG" +
                                                 "1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNl" +
                                                 "cnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3L" +
                                                 "WF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIi" +
                                                 "wicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlb" +
                                                 "nRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHM" +
                                                 "iLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbn" +
                                                 "RzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5h" +
                                                 "Z2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJ" +
                                                 "dfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYW" +
                                                 "Njb3VudCIsIm1hbmFnZS1hY2NvdW50LWx" +
                                                 "pbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6I" +
                                                 "lJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF9" +
                                                 "1c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIi" +
                                                 "wiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsIm" +
                                                 "ZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJ" +
                                                 "lYWxtYWRtaW5AZ21haWwuY29tIn0.D2" +
                                                 "nSVJbZ7M2JZosiC6z-HXx7-Rg1n7w7CCKvWtBzErJ" +
                                                 "VDIedvS5y6syxQnJbtl0yITYM4qP-gN0J" +
                                                 "i13qnwu0wjy-NorXvG7BOB5wl2SXekaaphXjv9e6N" +
                                                 "shQ5SEhyV1hMzfPRqLkZbpETjEOdPiMziG" +
                                                 "6k8sZCpast3c3diKb96dxjVIOhPayf2P9Z75b-qne" +
                                                 "gFuV1LkD9mIkGDyA7t5givfouskPSr09EK" +
                                                 "yxHf_m7kjPipy39cKODgcbsyYpwqAmHYaHJGsqIZ" +
                                                 "YDPTCjvzmkrZOQlGJ_sXAVmxrZY8psUZ7MKe" +
                                                 "Fd4l9xwvfi4N-3FFT5D4_tJq0Yp3RW5Bs3JVc1ig"
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
            val emisConfig = EmisConfiguration(config.emisApplicationId, "2.1.0.0")

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
                                        type = TppConstants.NationalIdTypeNhs,
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
                Patient.aderynCanon.odsCode,
                Patient.aderynCanon.patientId
        )

        val visionGetConfiguration = ServiceDefinition(
                VisionConstants.configurationName,
                VisionConstants.configurationVersion)

        val visionGetHistory = ServiceDefinition(
                VisionConstants.prescriptionHistory,
                VisionConstants.prescriptionHistoryVersion)

        val visionGetEligibleRepeats = ServiceDefinition(
                VisionConstants.eligibleRepeats,
                VisionConstants.eligibleRepeatsVersion)

        val visionOrderNewPrescription = ServiceDefinition(
                VisionConstants.newPrescription,
                VisionConstants.newPrescriptionVersion)

        val visionDemographicsConfiguration = ServiceDefinition(
                VisionConstants.demographicsName,
                VisionConstants.demographicsVersion)

        val visionConfigurationResponse = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(PatientNumber(number = patientVision.nhsNumbers[0])),
                name = getFullPatientName(patientVision)))

        val visionDemographicsResponse = Demographics(
                name = Name(patientVision.title, patientVision.firstName, patientVision.surname),
                gender = patientVision.sex.name,
                dateOfBirth = patientVision.dateOfBirth,
                primaryAddress = PrimaryAddress(houseName = patientVision.address.houseNameFlatNumber,
                        street = patientVision.address.numberStreet,
                        town  = patientVision.address.village + ", " + patientVision.address.town,
                        county = patientVision.address.county,
                        postcode = patientVision.address.postcode)
                )

        fun getFullPatientName(patient: Patient): String{
          return "${patient.title} ${patient.firstName} ${patient.surname}"
        }
    }
}
