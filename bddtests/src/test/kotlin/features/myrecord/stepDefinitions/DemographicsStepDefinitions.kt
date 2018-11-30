package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics

open class DemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @When("^I get the users demographic data$")
    fun whenIGetTheUsersDemographicsDataFor() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getDemographics()

            Serenity.setSessionVariable(Demographics::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Given("^the GP Practice has enabled demographics functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        DemographicsFactory.getForSupplier(getService).enabled(this@DemographicsStepDefinitions.patient)
    }

    @Given("^the GP Practice has disabled demographics functionality for (.*)$")
    fun butTheGPPracticeHasDisabledDemographicsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        DemographicsFactory.getForSupplier(getService).disabled(this@DemographicsStepDefinitions.patient)
    }

    @Then("^I receive the demographic object$")
    fun thenIReceiveADemographicObject() {
        val result = Serenity.sessionVariableCalled<Demographics>(Demographics::class)
        Assert.assertNotNull(result)
    }
}

