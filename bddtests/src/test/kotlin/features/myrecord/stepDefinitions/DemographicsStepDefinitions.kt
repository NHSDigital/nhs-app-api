package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import features.myrecord.factories.DemographicsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics

open class DemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled demographics functionality$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityFor() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        DemographicsFactory.getForSupplier(gpSystem).enabled(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has enabled demographics functionality for the current patient$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityForTheCurrentPatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        DemographicsFactory.getForSupplier(gpSystem).enabled(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled demographics functionality$")
    fun butTheGPPracticeHasDisabledDemographicsFunctionalityFor() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        DemographicsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @Given("^the demographics endpoint responds with (?:a|an) \"(.*)\" error$")
    fun theDemographicsEndpointRespondsWitSpecifiedError(expectedError: String) {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        if (expectedError.equals("internal server error")) {
            DemographicsFactory.getForSupplier(gpSystem).throwInternalError(SerenityHelpers.getPatient())
        }
        if (expectedError.equals("forbidden")) {
            DemographicsFactory.getForSupplier(gpSystem).throwForbiddenError(SerenityHelpers.getPatient())
        }
        if (expectedError.equals("bad gateway")) {
            DemographicsFactory.getForSupplier(gpSystem).throwForbiddenError(SerenityHelpers.getPatient())
        }
    }

    @Given("^the demographics endpoint responds with internal server error$")
    fun theDemographicsEndpointRespondsWithInternalServerError() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        DemographicsFactory.getForSupplier(gpSystem).throwInternalError(SerenityHelpers.getPatient())
    }

    @When("^I get the users demographic data$")
    fun whenIGetTheUsersDemographicsDataFor() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getDemographics(patientId)

            Serenity.setSessionVariable(Demographics::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive the demographic object$")
    fun thenIReceiveADemographicObject() {
        val result = Serenity.sessionVariableCalled<Demographics>(Demographics::class)
        Assert.assertNotNull(result)
    }
}

