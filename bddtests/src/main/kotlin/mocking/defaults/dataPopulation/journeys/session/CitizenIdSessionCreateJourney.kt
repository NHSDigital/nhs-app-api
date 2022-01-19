package mocking.defaults.dataPopulation.journeys.session

import config.Config
import constants.SessionConstants
import mocking.IdTokenBuilder
import mocking.MockingClient
import mocking.citizenId.login.UpliftLoginRequestBuilder
import mocking.citizenId.models.notifications.SuccessResponse
import mocking.citizenId.models.signingKeys.SucceededResponse
import models.Patient
import utils.GlobalSerenityHelpers
import utils.getOrFail
import utils.isTrueOrFalse
import utils.set
import utils.SerenityHelpers

private const val DELAY_BY = 1000L
class CitizenIdSessionCreateJourney {

    private val mockingClient = MockingClient.instance

    fun createFor(patient: Patient, alternativeUser:Boolean = false, hasNullToken: Boolean = false) {
        if (!GlobalSerenityHelpers.CITIZEN_ID_SESSION_CREATED.isTrueOrFalse() || alternativeUser) {
            val accessToken = createMockingSteps(patient)

            mockingClient.favicon()

            mockingClient.forCitizenId.mock {
                userInfoRequest(accessToken).respondWithSuccess(patient, true)
                        .inScenario("ON_DEMAND")
                        .willSetStateTo("SSO")
            }

            val nhsLoginPatientSubjectOverride =
                SerenityHelpers.getValueOrNull<String>("NHS_LOGIN_PATIENT_SUBJECT_OVERRIDE")

            mockingClient.forCitizenId.mock {
                userInfoRequest(accessToken)
                    .respondWithSuccess(patient, hasNullToken, nhsLoginPatientSubjectOverride)
                        .inScenario("ON_DEMAND")
                        .whenScenarioStateIs("SSO")
            }

            GlobalSerenityHelpers.CITIZEN_ID_SESSION_CREATED.set(true)
        }
    }

    fun createTermsNotAcceptedFor(patient: Patient) {
        createMockingSteps(patient)
        mockingClient.forCitizenId.mock {
            completeLoginRequest(patient).respondWithTermsNotAcceptedResponse()
        }
    }

    fun createInvalidFor(patient: Patient) {
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val idToken = mockingStepsInitialise(patient, redirectUri)
        mockingClient.forCitizenId.mock {
            tokenRequest(patient.codeVerifier, patient.authCode, redirectUri)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
        }
    }

    fun createTimeoutFor(patient: Patient) {
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val idToken = mockingStepsInitialise(patient, redirectUri)

        mockingClient.forCitizenId.mock {
            tokenRequest(patient.codeVerifier, patient.authCode, redirectUri)
                    .respondWithSuccess(accessToken = patient.accessToken, idToken = idToken)
                    .delayedBy(java.time.Duration.ofSeconds(DELAY_BY))
        }
    }

    private fun mockingStepsInitialise(patient: Patient, redirectUri: String): String {
        mockingClient.forCitizenId.mock {
            initialLoginRequest(patient, redirectUri, Config.instance.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            completeLoginRequest(patient)
                    .respondWithRedirectResponse()
        }

        val idToken = ""

        mockingClient.forCitizenId.mock {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }
        return idToken
    }

    private fun createMockingSteps(patient: Patient): String {
        val loginRedirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val gpSessionRedirectUri = GlobalSerenityHelpers.GP_SESSION_REDIRECT_URI.getOrFail<String>()

        mockingClient.forCitizenId.mock {
            initialLoginRequest(patient, loginRedirectUri, Config.instance.cidClientId)
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            loginWithAssertedLoginIdentityRequestBuilder(patient, loginRedirectUri, Config.instance.cidClientId)
                .respondWithRedirectBackToNhsAppPage()
        }

        mockingClient.forCitizenId.mock {
            ssoLoginRequest(patient, gpSessionRedirectUri, Config.instance.cidClientId)
                    .respondWithRedirectURI()
        }

        mockingClient.forCitizenId.mock {
            UpliftLoginRequestBuilder(patient, loginRedirectUri, Config.instance.cidClientId)
                    .respondWithUpliftPage()
        }

        mockingClient.forCitizenId.mock {
            createAccountRequest()
                    .respondWithLoginPage()
        }

        mockingClient.forCitizenId.mock {
            completeLoginRequest(patient)
                    .respondWithRedirectResponse()
        }

        val idToken = IdTokenBuilder().getSignedToken(patient).serialize()
        val accessToken = patient.accessToken

        mockingClient.forCitizenId.mock {
            configurationRequest()
                    .respondWithSuccess(SuccessResponse())
        }

        mockingClient.forCitizenId.mock {
            signingKeyRequest()
                    .respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }

        mockingClient.forCitizenId.mock {
            tokenRequest(patient.codeVerifier, patient.authCode, loginRedirectUri)
                    .respondWithSuccess(accessToken = accessToken, idToken = idToken)
        }

        mockingClient.forCitizenId.mock {
            tokenRequest(patient.codeVerifier, patient.authCode, gpSessionRedirectUri)
                    .respondWithNhsAppCredentialsSuccess(accessToken = accessToken, idToken = idToken)
        }

        mockingClient.forCitizenId.mock {
            accountSettingsRequest().respondWithSettingsPage(patient)
        }

        patient.refreshToken?.let {
            mockingClient.forCitizenId.mock {
                refreshTokenRequest(SessionConstants.RefreshToken)
                        .respondWithSuccess(accessToken = it)
            }
        }

        return accessToken
    }
}
