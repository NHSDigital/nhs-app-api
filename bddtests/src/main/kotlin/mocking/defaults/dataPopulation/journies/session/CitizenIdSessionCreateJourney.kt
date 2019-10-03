package mocking.defaults.dataPopulation.journies.session

import config.Config
import mocking.MockingClient
import mocking.citizenId.models.notifications.SuccessResponse
import mocking.citizenId.models.signingKeys.SucceededResponse
import models.Patient
import worker.models.patient.Im1ConnectionToken

private const val DELAY_BY = 1000L
class CitizenIdSessionCreateJourney(val mockingClient: MockingClient) {

    fun createFor(patient: Patient) {
        val accessToken = createMockingSteps(patient)

        mockingClient.forCitizenId {
            userInfoRequest(accessToken).respondWithSuccess(patient)
        }
    }

    fun createInvalidFor(patient: Patient) {
        val idToken = mockingStepsInitialise(patient)

        mockingClient.forCitizenId {
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }
    }

    fun createTimeoutfor(patient: Patient) {
        val idToken = mockingStepsInitialise(patient)

        mockingClient.forCitizenId {
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
                    .delayedBy(java.time.Duration.ofSeconds(DELAY_BY))
        }
    }

    private fun mockingStepsInitialise(patient: Patient): String {
        mockingClient.forCitizenId {
            initialLoginRequest(patient, Config.instance.cidRedirectUri, Config.instance.cidClientId)
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

    fun createInvalidAuthenticationTokenfor(patient: Patient) {
        val accessToken = createMockingSteps(patient)

        mockingClient.forCitizenId {
            userInfoRequest(accessToken).respondWithSuccess(patient
                    .copy(im1ConnectionToken =
                    Im1ConnectionToken("Invalid", "invalid")))
        }
    }

    private fun createMockingSteps(patient: Patient): String {
        mockingClient.forCitizenId {
            initialLoginRequest(patient, Config.instance.cidRedirectUri, Config.instance.cidClientId)
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
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = accessToken, idToken = idToken)
        }
        return accessToken
    }
}
