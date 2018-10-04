package features.authentication.stepDefinitions

import features.appointments.data.AppointmentsBookingData.Companion.mockingClient
import mocking.vision.VisionMockDefaults
import models.Patient
import java.time.Duration

class AuthenticationFactoryVision  : AuthenticationFactory("VISION"){

    override fun patientDoesNotExist(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Details")
    }

    override fun patientWithIncorrectLinkageKey(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Details")
    }

    override fun patientWithIncorrectSurname(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Details")
    }

    override fun patientWithIncorrectDOB(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Details")
    }

    override fun patientWithSurnameInWrongFormat(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Details")
    }

    override fun patientWithAccountIDInWrongFormat(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Parameter")
    }

    override fun patientWithLinkageKeyInWrongFormat(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Parameter")
    }

    override fun patientWithDOBInWrongFormat(patient: Patient) {
        createInvalidTestForVision(patient, "Invalid Parameter")
    }

    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient
                .forVision {
                    getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionMockDefaults.visionConfigurationResponse)
                            .delayedBy(Duration.ofSeconds(31))
                }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient
                .forVision {
                    getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWitInvalidUserCredentials()
                }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forVision {
            getConfigurationRequest(
                    VisionMockDefaults.getVisionUserSession(patient))
                    .respondWithServiceUnavailable()
        }
    }

    companion object {

        fun patientIsAlreadyRegistered(patient: Patient) {
            createInvalidTestForVision(patient, "Already Registered")
        }

        fun patientHasALockedAccount(patient: Patient) {
            createInvalidTestForVision(patient, "Patient Locked")
        }

        fun createInvalidTestForVision(patient: Patient, typeOfError : String) {
            mockingClient.forVision {
                getRegisterRequest(
                        VisionMockDefaults.getVisionUserSession(patient),
                        patient)
                        .respondWithError(typeOfError)
            }

            mockingClient.forVision {
                getConfigurationRequest(
                        VisionMockDefaults.getVisionUserSession(patient))
                        .respondWithSuccess(
                                VisionMockDefaults.visionConfigurationResponse)
            }
        }
    }
}
