package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProblemsFactory
import mocking.data.myrecord.ProblemsData
import net.serenitybdd.core.Serenity
import constants.DateTimeFormats
import org.junit.Assert.assertEquals
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
open class V1MedicalRecordProblemsStepDefinitions {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^the GP Practice has enabled problems functionality$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled problems functionality$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }
    @Given("^no Problems records exist for the patient$")
    fun givenNoProblemsRecordsExistForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @When("there is bad problems data returned")
    fun badProblemsDataReturned() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).badDataResponse(SerenityHelpers.getPatient())
    }

    @Given("^there is an error retrieving Problems data$")
    fun givenThereIsAnErrorRetrievingProblemsData() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("^the EMIS GP Practice has three problem results where the second record has no date$")
    fun givenTheEMISGPPracticeHasThreeProblemResultsWhereTheSecondRecordHasNoDate() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).secondProblemHasNoDate(SerenityHelpers.getPatient())
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
    fun thenIReceiveAProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @Then("^I see health condition records displayed - Medical Record v1$")
    fun thenISeeProblemsRecordsDisplayedV1() {
        assertEquals(
                NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED,
                medicalRecordV1Page.healthConditions.allRecordItems().count())
    }

    @Then("^I see the expected Health conditions displayed with unknown date for the third result$")
    fun thenISeeTheExpectedHealthConditionsDisplayedWithUnknownDateForThirdResult() {
        val expectedProblems =
                ProblemsData.getEmisProblemRecordsWhereTheSecondRecordHasNoEffectiveDate().medicalRecord.problems
        val sortedExpectedProblems = expectedProblems.sortedByDescending { it.observation.effectiveDate?.value }

        val onScreenProblems = medicalRecordV1Page.healthConditions.allRecordItems()
        assertEquals(expectedProblems.size, onScreenProblems.count())

        for (i in onScreenProblems.indices) {
            if (i == expectedProblems.lastIndex) {
                assertEquals("Unknown Date", onScreenProblems[i].label)
            } else {
                val expectedDate = (sortedExpectedProblems[i].observation.effectiveDate?.value)?.takeWhile {
                    !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenProblems[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                assertEquals(expectedDate, actualDate)
            }

        }
    }
}
