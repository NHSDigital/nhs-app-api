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
            createAccountRequest(patient = patient)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId {
            completeLoginRequest(patient)
                    .respondWithRedirectResponse()
        }

        mockingClient.forCitizenId {
            tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                    .respondWithSuccess(accessToken = patient.accessToken)
        }

        mockingClient.forCitizenId {
            userInfoRequest("Bearer ${patient.accessToken}")
                    .respondWithSuccess(patient)
        }
    }
}