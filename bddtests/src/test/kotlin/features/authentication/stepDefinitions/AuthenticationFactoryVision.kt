package features.authentication.stepDefinitions

import mocking.vision.VisionMockDefaults
import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import java.time.Duration

private const val DELAY_BY_SECONDS = 31L

class AuthenticationFactoryVision : AuthenticationFactory("VISION") {

    override fun patientWithIncompleteResponse(patient: Patient) {
        Assert.fail("NHSO-3484")
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

    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient
                .forVision {
                    getConfigurationRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionMockDefaults.visionConfigurationResponse)
                            .delayedBy(Duration.ofSeconds(DELAY_BY_SECONDS))
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
        val mockingClient = SerenityHelpers.getMockingClient()

        fun patientIsAlreadyRegistered(patient: Patient) {
            createInvalidTestForVision(patient, "Already Registered")
        }

        fun patientHasALockedAccount(patient: Patient) {
            createInvalidTestForVision(patient, "Patient Locked")
        }

        fun createInvalidTestForVision(patient: Patient, typeOfError: String) {
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
