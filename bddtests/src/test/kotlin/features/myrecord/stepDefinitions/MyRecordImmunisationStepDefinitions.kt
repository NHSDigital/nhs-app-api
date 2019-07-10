package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ImmunisationsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

open class MyRecordImmunisationStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled immunisations functionality and multiple immunisation records exist$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleRecordsExist() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^no immunisation records exist for the patient$")
    fun givenNoImmunisationRecordsExistForThePatient() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the user does not have access to view immunisations$")
    fun givenUserDoesNotHaveAccessToViewImmunisations() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^there is an error retrieving immunisations data$")
    fun givenThereIsAnErrorRetrievingImmunisationsData() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ImmunisationsFactory.getForSupplier(getService).errorRetrieving(SerenityHelpers.getPatient())
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

    @Then("^I see the expected immunisations displayed$")
    fun thenISeeTheExpectedImmunisationsDisplayed() {
        val expectedImmunisations = ImmunisationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedImmunisations()

        val onScreenImmunisations = myRecordInfoPage.immunisations.allRecordItems()

        Assert.assertEquals(expectedImmunisations.count(), onScreenImmunisations.count())

        for (i in onScreenImmunisations.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            expectedImmunisations[i].effectiveDate.value),
                    LocalDate.parse(
                            onScreenImmunisations[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    ))

            Assert.assertEquals(expectedImmunisations[i].term, onScreenImmunisations[i].bodyElements[0])
            Assert.assertEquals(expectedImmunisations[i].nextDate, onScreenImmunisations[i].bodyElements[1])
            Assert.assertEquals(expectedImmunisations[i].status, onScreenImmunisations[i].bodyElements[2])
        }
    }
}
