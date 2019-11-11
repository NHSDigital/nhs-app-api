package features.sharedSteps.backend

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.set
import worker.WorkerClient
import java.util.concurrent.TimeUnit

private const val ADDITIONAL_TIME_TO_DELAY = 10
open class SharedStepDefinitionsBackend{

    val mockingClient = MockingClient.instance

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)

        GlobalSerenityHelpers.GP_SYSTEM.set(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)

        givenIHaveLoggedInAndHaveAValidSessionCookie()
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun givenIHaveLoggedInAndHaveAValidSessionCookie() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatientOrNull()
                ?: Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession)
        val patientConfig = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getPatientLinkedAccountsConfiguration()
        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(patientConfig.response.id)
    }

    @When("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        val delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + ADDITIONAL_TIME_TO_DELAY)
    }
}