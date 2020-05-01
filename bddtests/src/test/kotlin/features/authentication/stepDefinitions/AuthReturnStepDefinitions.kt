package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.emis.practices.SettingsResponseModel
import models.Patient
import models.patients.EmisPatients
import models.patients.TppPatients
import net.thucydides.core.annotations.Steps

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

    @Given("^I am logged into Citizen ID but GP System authentication fails$")
    fun loggedInInCitizenIdGpAuthenticationFails() {
        this.patient = TppPatients.kevinBarry

        CitizenIdSessionCreateJourney().createInvalidAuthenticationTokenFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but EMIS session cannot be established$")
    fun loggedInInCitizenIdSessionNotEstablished() {
        this.patient = EmisPatients.montelFrye

        CitizenIdSessionCreateJourney().createFor(patient)
        mockingClient.forEmis {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)
        }
        mockingClient.forEmis {
            practiceSettingsRequest(patient)
                    .respondWithSuccess( SettingsResponseModel())
        }
        mockingClient.forEmis {
            authentication.sessionRequest(patient).respondWithServerError()
        }
        browser.goToApp()
        login.using(this.patient)
    }
}
