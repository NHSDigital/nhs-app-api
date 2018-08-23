package mocking.stubs

import config.Config
import mocking.MockingClient
import mocking.citizenId.models.TokenRequest
import mocking.citizenId.models.signingKeys.SuceededResponse
import models.Patient

class CitizenIdStubs(private val mockingClient: MockingClient) {

    fun createFor(patient: Patient) {

        mockingClient.forCitizenId {
            initialLoginRequest(patient,".*", Config.instance.cidClientId,"matches")
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest(patient,patient.surname)
                    .respondWithRedirectResponse()
        }

        val idToken = Patient.getIdToken(patient)

        mockingClient.forCitizenId {
            signingKeyRequest()
                    .respondWithSuccess(SuceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId {
            val codeVerifier = patient.cidUserSession.codeVerifier
            val authCode = patient.cidUserSession.authCode
            tokenRequest(codeVerifier, authCode, TokenRequest(codeVerifier, code = authCode,redirectUri = ".*"))
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }
    }
}