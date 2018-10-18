package mocking.defaults

import mocking.MockingClient
import mocking.emis.EmisConfiguration
import config.Config
import mocking.MockingConfiguration
import mocking.vision.Demographics.Demographics
import mocking.vision.Demographics.Name
import mocking.vision.Demographics.PrimaryAddress
import mocking.vision.VisionConstants
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.Prescriptions
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient

class MockDefaults {

    companion object {
        const val DEFAULT_END_USER_SESSION_ID: String = "7YjG1LYkOkSY1iAcXGG8ZU"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"
        const val DEFAULT_ODS_CODE: String = "A29928"
        const val DEFAULT_ODS_CODE_VISION: String = "X00100"


        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration(config.emisApplicationId, "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }

        val patient = Patient.getDefault("EMIS")
        val patientVision = Patient.getDefault("VISION")


        val visionUserSession = VisionUserSession(
                Patient.aderynCanon.rosuAccountId,
                Patient.aderynCanon.apiKey,
                Patient.aderynCanon.odsCode,
                Patient.aderynCanon.patientId
        )

        val visionUserSessionPrescriptionDisabled = VisionUserSession(
                Patient.aderynCanon.rosuAccountId,
                Patient.aderynCanon.apiKey,
                Patient.aderynCanon.odsCode,
                Patient.aderynCanon.patientId,
                isRepeatPrescriptionsEnabled = false
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

        val visionConfigurationResponsePrescriptionsDisabled = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(
                        PatientNumber(number = patientVision.nhsNumbers[0])
                ),
                name = getFullPatientName(patientVision)),
                prescriptions = Prescriptions(false))

        fun getFullPatientName(patient: Patient): String{
          return "${patient.title} ${patient.firstName} ${patient.surname}"
        }
    }
}
