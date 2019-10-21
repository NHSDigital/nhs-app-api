package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProblemsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
open class MyRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled problems functionality$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityFor() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        ProblemsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled problems functionality$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        ProblemsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }
    @Given("^no Problems records exist for the patient$")
    fun givenNoProblemsRecordsExistForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        ProblemsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^there is an error retrieving Problems data$")
    fun givenThereIsAnErrorRetrievingProblemsData() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        ProblemsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @When("^the flag informing that the patient has access to the problem data is set to \"(.*)\"$")
    fun andHasAccessToProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the problem data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasErrored)
    }

    @Then("^I receive \"(.*)\" problems as part of the my record object$")
    fun thenIReceiveAnProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @Then("^I see health condition records displayed$")
    fun thenISeeProblemsRecordsDisplayed() {
        assertEquals(NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED, myRecordInfoPage.healthConditions.allRecordItems().count())
    }

    @Then("^I see the expected health conditions displayed$")
    fun thenISeeTheExpectedProblemsDisplayed() {
        val expectedProblems = ProblemsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedProblems()

        val onScreenProblems = myRecordInfoPage.healthConditions.allRecordItems()

        Assert.assertEquals(expectedProblems.count(), onScreenProblems.count())

        for (i in onScreenProblems.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            expectedProblems[i].effectiveDate.value),
                    LocalDate.parse(
                            onScreenProblems[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    ))

            for (j in expectedProblems[i].lineItems.indices) {
                Assert.assertEquals(expectedProblems[i].lineItems[j].text, onScreenProblems[i].bodyElements[j])
            }
        }
    }
}
