package mocking.defaults.dataPopulation.journies.session

import config.Config
import mocking.MockingClient
import mocking.citizenId.models.signingKeys.SucceededResponse
import models.Patient

class CitizenIdSessionCreateJourney(val mockingClient: MockingClient) {

    fun createFor(patient: Patient) {
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

        mockingClient.forCitizenId {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId {
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }

        mockingClient.forCitizenId {
            userInfoRequest(patient.accessToken).respondWithSuccess(patient)
        }
    }

    fun createInvalidFor(patient: Patient) {
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

        mockingClient.forCitizenId {
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }
    }
}
