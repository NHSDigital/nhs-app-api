package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics

open class DemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled demographics functionality$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityFor() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        DemographicsFactory.getForSupplier(getService).enabled(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled demographics functionality$")
    fun butTheGPPracticeHasDisabledDemographicsFunctionalityFor() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        DemographicsFactory.getForSupplier(getService).disabled(SerenityHelpers.getPatient())
    }

    @When("^I get the users demographic data$")
    fun whenIGetTheUsersDemographicsDataFor() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getDemographics()

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

