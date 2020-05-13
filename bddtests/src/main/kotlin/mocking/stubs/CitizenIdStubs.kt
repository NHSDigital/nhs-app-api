package mocking.stubs

import config.Config
import mocking.IdTokenBuilder
import mocking.MockingClient
import mocking.citizenId.models.TokenRequest
import mocking.citizenId.models.signingKeys.SucceededResponse
import models.Patient

class CitizenIdStubs(private val mockingClient: MockingClient) {

    fun createFor(patient: Patient) {

        mockingClient.forCitizenId.mock {
            initialLoginRequest(patient,".*", Config.instance.cidClientId,"matches")
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            completeLoginRequest(patient,patient.name.surname)
                    .respondWithRedirectResponse()
        }

        val idToken = IdTokenBuilder().getSignedToken(patient).serialize()

        mockingClient.forCitizenId.mock {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId.mock {
            val codeVerifier = patient.codeVerifier
            val authCode = patient.authCode
            tokenRequest(codeVerifier, authCode, ".*", customTokenRequest = TokenRequest(codeVerifier,
                    code = authCode,
                    redirectUri = ".*"))
                    .respondWithSuccess(patient.accessToken, idToken = idToken)
        }

        mockingClient.forCitizenId.mock {
            userInfoRequest(patient.accessToken).respondWithSuccess(patient)
        }
    }
}
