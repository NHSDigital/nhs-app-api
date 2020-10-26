package features.authentication.stepDefinitions

import constants.ErrorResponseCodeTpp.LOGIN_PROBLEM
import io.cucumber.java.en.Given
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.defaults.TppMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.emis.practices.SettingsResponseModel
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.Error
import models.Patient
import models.patients.EmisPatients
import models.patients.TppPatients
import net.thucydides.core.annotations.Steps
import utils.GlobalSerenityHelpers
import utils.set
import java.util.UUID

class AuthReturnStepDefinitions {
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps

    private lateinit var patient: Patient

    private val mockingClient = MockingClient.instance

    @Given("^I am logged into Citizen ID but am receiving invalid data$")
    fun loggedInInCitizenIdInvalidData() {
        this.patient = EmisPatients.paulSmith

        CitizenIdSessionCreateJourney().createInvalidFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but EMIS session cannot be established$")
    fun loggedInInCitizenIdSessionNotEstablished() {
        this.patient = EmisPatients.montelFrye
        GlobalSerenityHelpers.PATIENT.set(this.patient.withoutGPSupplierConnection())

        CitizenIdSessionCreateJourney().createFor(patient)
        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)
        }
        mockingClient.forEmis.mock {
            practiceSettingsRequest(patient)
                    .respondWithSuccess( SettingsResponseModel())
        }
        mockingClient.forEmis.mock {
            authentication.sessionRequest(patient).respondWithServerError()
        }
        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but TPP session returns error code 9$")
    fun loggedInInCitizenIdTppErrorCode9() {
        this.patient = TppPatients.kevinBarry

        CitizenIdSessionCreateJourney().createFor(patient)

        mockingClient.forTpp.mock {
            authentication.authenticateRequest(
                Authenticate(
                    apiVersion = TppMockDefaults.TPP_API_VERSION,
                    accountId = patient.accountId,
                    passphrase = patient.passphrase,
                    unitId = patient.odsCode,
                    uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                    application = Application(
                        name = "NhsApp",
                        version = "1.0",
                        providerId = TppMockDefaults.DEFAULT_TPP_PROVIDER_ID,
                        deviceType = "NhsApp"
                    )
                )
            ).respondWithError(
                Error(
                  LOGIN_PROBLEM,
                   "Problem logging on",
                  UUID.randomUUID().toString()
                )
            )
        }

        browser.goToApp()
        login.using(this.patient)
    }
}
