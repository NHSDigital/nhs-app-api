package mocking.defaults

import mocking.vision.VisionConstants
import mocking.vision.demographics.Demographics
import mocking.vision.demographics.Name
import mocking.vision.demographics.PrimaryAddress
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.Prescriptions
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.Appointments
import models.Patient

class VisionMockDefaults{

    companion object {

        const val DEFAULT_ODS_CODE_VISION: String = "X00100"

        val patientVision = Patient.getDefault("VISION")

        val visionGetConfiguration = ServiceDefinition(
                VisionConstants.configurationName,
                VisionConstants.configurationVersion)

        val visionGetPrescriptionHistory = ServiceDefinition(
                VisionConstants.prescriptionHistory,
                VisionConstants.prescriptionHistoryVersion)

        val visionGetEligibleRepeats = ServiceDefinition(
                VisionConstants.eligibleRepeats,
                VisionConstants.eligibleRepeatsVersion)

        val visionDemographicsConfiguration = ServiceDefinition(
                VisionConstants.demographicsName,
                VisionConstants.demographicsVersion)

        val visionGetRegister = ServiceDefinition(
                VisionConstants.linkAccount,
                VisionConstants.linkAccountVersion)

        val visionOrderNewPrescription = ServiceDefinition(
                VisionConstants.newPrescription,
                VisionConstants.newPrescriptionVersion)

        val visionUserSessionPrescriptionDisabled = VisionUserSession(
                patientVision.rosuAccountId,
                patientVision.apiKey,
                patientVision.odsCode,
                patientVision.patientId,
                isRepeatPrescriptionsEnabled = false
        )

        val visionUserSession = VisionUserSession(
                patientVision.rosuAccountId,
                patientVision.apiKey,
                patientVision.odsCode,
                patientVision.patientId
        )

        val visionConfigurationResponsePrescriptionsDisabled = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(
                        PatientNumber(number = patientVision.nhsNumbers[0])
                ),
                name = patientVision.formattedFullName()),
                prescriptions = Prescriptions(false))

        val visionConfigurationResponse = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(PatientNumber(number = patientVision.nhsNumbers[0])),
                name = patientVision.formattedFullName()))

        val visionDemographicsResponse = Demographics(
                name = Name(patientVision.title, patientVision.firstName, patientVision.surname),
                gender = patientVision.sex.name,
                dateOfBirth = patientVision.dateOfBirth,
                primaryAddress = PrimaryAddress(houseName = patientVision.address.houseNameFlatNumber,
                        street = patientVision.address.numberStreet,
                        town = patientVision.address.village + ", " + patientVision.address.town,
                        county = patientVision.address.county,
                        postcode = patientVision.address.postcode)
        )

        fun getVisionUserSession(patient: Patient): VisionUserSession {
            return VisionUserSession(
                    patient.rosuAccountId,
                    patient.apiKey,
                    patient.odsCode,
                    patient.patientId)
        }

        val visionConfigurationResponseAppointmentsDisabled = Configuration(account = Account(patientVision.patientId,
                patientNumber = listOf(
                        PatientNumber(number = patientVision.nhsNumbers[0])
                ),
                name = patientVision.formattedFullName()),
                appointments = Appointments(false))
    }
}
