package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ImmunisationsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordImmunisationStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled immunisations functionality and multiple immunisation records exist for (.*)$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleRecordsExistFor(getService: String) {
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).enabledWithRecords(patient)
    }

    @Given("^no immunisation records exist for the patient for (.*)$")
    fun givenNoImmunisationRecordsExistForThePatientFor(getService: String) {
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).enabledWithBlankRecord(patient)
    }

    @Given("^the user does not have access to view immunisations for (.*)$")
    fun givenUserDoesNotHaveAccessToViewImmunisationsFor(getService: String) {
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).noAccess(patient)
    }

    @Given("^there is an error retrieving immunisations data for (.*)$")
    fun givenThereIsAnErrorRetrievingImmunisationsDatafor(getService: String) {
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).errorRetrieving(patient)
    }

    @When("^I get the users immunisations$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" immunisations as part of the my record object$")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.immunisations.data.count())
    }

    @Then("^I see immunisation records displayed$")
    fun thenISeeImmunisationRecordsDisplayed() {
        assertEquals(2, myRecordInfoPage.immunisations.allRecordItems().count())
    }
}
