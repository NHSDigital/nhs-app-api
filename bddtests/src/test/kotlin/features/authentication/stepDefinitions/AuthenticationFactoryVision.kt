package features.authentication.stepDefinitions

import mocking.defaults.VisionMockDefaults
import models.Patient
import utils.SerenityHelpers
import java.time.Duration

class AuthenticationFactoryVision : AuthenticationFactory("VISION") {

    override fun patientWithIncompleteResponse(patient: Patient) {
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithCorruptedContent()
                }
    }

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

    override fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long) {
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionMockDefaults.visionConfigurationResponse)
                            .delayedBy(Duration.ofSeconds(delayBySeconds))
                }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithInvalidUserCredentials()
                }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forVision {
            authentication.getConfigurationRequest(
                    VisionMockDefaults.getVisionUserSession(patient))
                    .respondWithServiceUnavailable()
        }
    }

    companion object {
        val mockingClient = SerenityHelpers.getMockingClient()

        fun patientIsAlreadyRegistered(patient: Patient) {
            createInvalidTestForVision(patient, "Already Registered")
        }

        fun patientHasALockedAccount(patient: Patient) {
            createInvalidTestForVision(patient, "Patient Locked")
        }

        fun createInvalidTestForVision(patient: Patient, typeOfError: String) {
            mockingClient.forVision {
                authentication.getRegisterRequest(
                        VisionMockDefaults.getVisionUserSession(patient),
                        patient)
                        .respondWithError(typeOfError)
            }

            mockingClient.forVision {
                authentication.getConfigurationRequest(
                        VisionMockDefaults.getVisionUserSession(patient))
                        .respondWithSuccess(
                                VisionMockDefaults.visionConfigurationResponse)
            }
        }
    }
}
