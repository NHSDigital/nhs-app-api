package features.sharedSteps.backend

import constants.Supplier
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import java.util.*

open class SharedStepDefinitionsBackend{

    val mockingClient = MockingClient.instance

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        givenIHaveLoggedInAndHaveAValidSessionCookie()
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun givenIHaveLoggedInAndHaveAValidSessionCookie() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatientOrNull()
                ?: Patient.getDefault(gpSystem)
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.generateUserSessionRequest(redirectUri))

        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enableForPatientProxyAccounts(patient)

        val patientConfig = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getPatientLinkedAccountsConfiguration()
        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(patientConfig.id)
    }

    @But("the patient id sent in the request is not valid")
    fun thePatientIdSendInTheRequestIsNotValid() {
        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(UUID.randomUUID().toString())
    }
}