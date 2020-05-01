package mocking.stubs

import config.Config
import mocking.IdTokenBuilder
import mocking.MockingClient
import mocking.citizenId.models.TokenRequest
import mocking.citizenId.models.signingKeys.SucceededResponse
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
            completeLoginRequest(patient,patient.name.surname)
                    .respondWithRedirectResponse()
        }

        val idToken = IdTokenBuilder().getSignedToken(patient).serialize()

        mockingClient.forCitizenId {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId {
            val codeVerifier = patient.codeVerifier
            val authCode = patient.authCode
            tokenRequest(codeVerifier, authCode, ".*", customTokenRequest = TokenRequest(codeVerifier,
                    code = authCode,
                    redirectUri = ".*"))
                    .respondWithSuccess(patient.accessToken, idToken = idToken)
        }

        mockingClient.forCitizenId {
            userInfoRequest(patient.accessToken).respondWithSuccess(patient)
        }
    }
}
