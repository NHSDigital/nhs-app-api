package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.AllergiesFactory
import features.myrecord.factories.MyRecordVisionMocker
import mocking.data.myrecord.AllergiesData
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.allergiesView
import net.serenitybdd.core.Serenity
import org.junit.Assert
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_ALLERGIES = 5

open class MyRecordAllergiesStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergies(count: Int) {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        AllergiesFactory.getForSupplier(getService).enabledWithRecords(SerenityHelpers.getPatient(), count)
    }

    @Given("^the GP Practice has enabled allergies functionality and has a drug and non drug allergy " +
            "record for VISION$")
    fun theGPPracticeHasEnabledAllergiesFunctionalityAndThePatientHasADrugAndNonDrugAllergyRecord() {
        setPatientToDefaultFor("VISION")
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesDrugAndNonDrugData()) }
    }

    @Given("the GP Practice has enabled allergies functionality and has 5 different " +
            "allergies with different date formats")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndHasFiveDifferentAllergiesWithDifferentDateFormats() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        val patient = SerenityHelpers.getPatient()
        when (getService) {
            "EMIS" ->
                mockingClient.forEmis {
                    myRecord.allergiesRequest(patient)
                            .respondWithSuccess(AllergiesData.getEmisAllergyRecordsWithDifferentDateParts())
                }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithSuccess(AllergiesData.getTppAllergiesData(NUMBER_OF_ALLERGIES))
                }
            }
        }
    }

    @Given("the GP Practice has disabled allergies functionality")
    fun butTheGPPracticeHasDisabledAllergiesFunctionalityForService() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        AllergiesFactory.getForSupplier(getService).disabled(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting allergies for VISION$")
    fun thereIsAnUnknownErrorGettingAllergiesFor() {
        setPatientToDefaultFor("VISION")
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat )
        { request -> request.respondWithUnknownError() }
    }

    @Then("^I see a message informing me to contact my GP for this information$")
    fun thenISeeAMessageInformingMeToContactMyGP() {
        myRecordInfoPage.noRecordsOrNoAccessParagraph.assertIsVisible()
    }

    @Then("^I do not see a message informing me to contact my GP for this information$")
    fun thenIDoNotSeeAMessageInformingMeToContactMyGP() {
        myRecordInfoPage.noRecordsOrNoAccessParagraph.assertElementNotPresent()
    }

    @When("^the flag informing that the patient has access to the allergy data is set to \"(.*)\"$")
    fun andHasAccessToAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the allergy data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasErrored)
    }

    @Then("^I receive \"(.*)\" allergies as part of the my record object$")
    fun thenIReceiveAnAllergiesObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.allergies.data.count())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    fun thenISeeOneOrMoreDrugTypeAllergiesRecordDisplayed() {
        Assert.assertEquals(2, myRecordInfoPage.allergies.allRecordItems().count())
        val expected = ArrayList<String>()
        for (i in 1..2) {
            expected.add(AllergiesData.TERM)
        }

        Assert.assertArrayEquals(expected.toArray(), myRecordInfoPage.allergies.allRecordItemBodies()
                .toTypedArray())
    }

    @Then("^I see the expected allergies displayed$")
    fun thenISeeTheExpectedAllergiesDisplayed() {
        val getService = SerenityHelpers.getGpSupplier()
        val allergyData = AllergiesFactory
                .getForSupplier(getService)
                .getExpectedAllergies()

        val onScreenAllergies = myRecordInfoPage.allergies.allRecordItems()

        Assert.assertEquals(
                allergyData.count(),
                onScreenAllergies.count())

        for (i in onScreenAllergies.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            allergyData[i].date.value
                    ),
                    LocalDate.parse(
                            onScreenAllergies[i].label,
                            DateTimeFormatter.ofPattern(
                                    DateTimeFormats.frontendBasicDateFormat)
                    )
            )

            // assuming 1 allergy per date for this method until needs expanded
            Assert.assertEquals(allergyData[i].name, onScreenAllergies[i].bodyElements.joinToString())
        }
    }

    @Then("^I see 5 allergies with different date formats$")
    fun thenISeeFiveAllergiesWithDifferentDateFormats() {

        Assert.assertEquals(NUMBER_OF_ALLERGIES, myRecordInfoPage.allergies.allRecordItems().count())
        val dates = myRecordInfoPage.allergies.allRecordItemLabels()

        assertContains(dates, "15 May 2018")
        assertContains(dates, "15 May 2018")
        assertContains(dates, "May 2018")
        assertContains(dates, "2018")
        assertContains(dates, "15 May 2018 09:52")
    }

    private fun assertContains(actualDates: List<String>, expected: String) {

        Assert.assertTrue("Expected to contain $expected, but was ${actualDates.joinToString()}",
                actualDates.contains(expected))
    }

    @Then("^I see a drug and non drug allergy record from VISION$")
    fun thenISeeADrugAndNonDrugAllergyRecordFromVision() {
        val allergyMessages = myRecordInfoPage.allergies.allRecordItemBodies()
        val expectedMessages = listOf(
                "H/O: drug allergy",
                "Paracetamol 500mg capsules",
                "Leg swelling",
                "Pollen"
        )
        Assert.assertTrue("Expected records", allergyMessages.size == expectedMessages.size)
        allergyMessages.forEachIndexed { i, message -> Assert.assertTrue(message == expectedMessages[i]) }
    }
}

