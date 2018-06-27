package mocking.defaults.dataPopulation.journies.session

import config.Config
import mocking.MockingClient
import mocking.defaults.MockDefaults
import models.Patient

class CitizenIdSessionCreateJourney(val mockingClient: MockingClient) {

    fun createFor(patient: Patient) {
        mockingClient.forCitizenId {
            initialLoginRequest(Config.instance.cidRedirectUri, Config.instance.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest()
                    .respondWithRedirectResponse(MockDefaults.userSessionRequest.authCode!!)
        }

        mockingClient.forCitizenId {
            tokenRequest(MockDefaults.userSessionRequest.codeVerifier, MockDefaults.userSessionRequest.authCode)
                    .respondWithSuccess()
        }

        mockingClient.forCitizenId {
            userInfoRequest("Bearer access_token")
                    .respondWithSuccess(patient)
        }
    }
}