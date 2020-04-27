package mocking.defaults.dataPopulation.journies.session

import config.Config
import mocking.MockingClient
import mocking.citizenId.login.UpliftLoginRequestBuilder
import mocking.citizenId.models.notifications.SuccessResponse
import mocking.citizenId.models.signingKeys.SucceededResponse
import models.Patient
import utils.GlobalSerenityHelpers
import utils.getOrFail
import utils.isTrueOrFalse
import utils.set
import worker.models.patient.Im1ConnectionToken

private const val DELAY_BY = 1000L
class CitizenIdSessionCreateJourney(val mockingClient: MockingClient) {

    fun createFor(patient: Patient, alternativeUser:Boolean = false) {
        if (!GlobalSerenityHelpers.CITIZEN_ID_SESSION_CREATED.isTrueOrFalse() || alternativeUser) {
            val accessToken = createMockingSteps(patient, GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail())
            mockingClient.forCitizenId {
                userInfoRequest(accessToken).respondWithSuccess(patient)
            }
            GlobalSerenityHelpers.CITIZEN_ID_SESSION_CREATED.set(true)
        }
    }

    fun createInvalidFor(patient: Patient) {
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val idToken = mockingStepsInitialise(patient, redirectUri)
        mockingClient.forCitizenId {
            tokenRequest(patient.codeVerifier, patient.authCode, redirectUri)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }
    }

    fun createTimeoutFor(patient: Patient) {
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val idToken = mockingStepsInitialise(patient, redirectUri)

        mockingClient.forCitizenId {
            tokenRequest(patient.codeVerifier, patient.authCode, redirectUri)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
                    .delayedBy(java.time.Duration.ofSeconds(DELAY_BY))
        }
    }

    private fun mockingStepsInitialise(patient: Patient, redirectUri: String): String {
        mockingClient.forCitizenId {
            initialLoginRequest(patient, redirectUri, Config.instance.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest(patient)
                    .respondWithRedirectResponse()
        }

        val idToken = ""

        mockingClient.forCitizenId {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }
        return idToken
    }

    fun createInvalidAuthenticationTokenFor(patient: Patient) {
        val accessToken = createMockingSteps(patient, GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail())

        mockingClient.forCitizenId {
            userInfoRequest(accessToken).respondWithSuccess(patient
                    .copy(im1ConnectionToken =
                    Im1ConnectionToken("Invalid", "invalid")))
        }
    }

    private fun createMockingSteps(patient: Patient, redirectUri :String): String {
        mockingClient.forCitizenId {
            initialLoginRequest(patient, redirectUri, Config.instance.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            UpliftLoginRequestBuilder(patient, redirectUri, Config.instance.cidClientId)
                    .respondWithUpliftPage()
        }

        mockingClient.forCitizenId {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest(patient)
                    .respondWithRedirectResponse()
        }

        val idToken = Patient.getIdToken(patient)
        val accessToken = patient.accessToken

        mockingClient.forCitizenId {
            configurationRequest()
                    .respondWithSuccess(SuccessResponse())
        }

        mockingClient.forCitizenId {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId {
            tokenRequest(patient.codeVerifier, patient.authCode, redirectUri)
                    .respondWithSuccess(accessToken = accessToken, idToken = idToken)
        }
        return accessToken
    }
}
