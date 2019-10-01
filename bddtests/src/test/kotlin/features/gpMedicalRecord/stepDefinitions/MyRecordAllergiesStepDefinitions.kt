package features.gpMedicalRecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.AllergiesFactory
import features.myrecord.factories.MyRecordVisionMocker
import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.data.myrecord.NUMBER_OF_ALLERGY_RECORDS
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.allergiesView
import net.serenitybdd.core.Serenity
import org.junit.Assert
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.gpMedicalRecord.AllergiesAndReactionsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_ALLERGIES = 5

open class MyRecordAllergiesStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var allergiesAndReactionsPage: AllergiesAndReactionsPage

    @Given("^the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies " +
            "- GP Medical Record$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergiesGpMedicalRecord(count: Int) {
        val getService = SerenityHelpers.getGpSupplier()
        AllergiesFactory.getForSupplier(getService).enabledWithRecords(SerenityHelpers.getPatient(), count)
    }

    @Given("^the GP Practice has enabled allergies functionality and has a drug and non drug allergy " +
            "record for VISION - GP Medical Record$")
    fun theGPPracticeHasEnabledAllergiesFunctionalityAndThePatientHasADrugAndNonDrugAllergyRecordGpMedicalRecord() {
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesDrugAndNonDrugData()) }
    }

    @Given("^the GP Practice has enabled allergies functionality and has a drug and non drug allergy " +
            "record for EMIS - GP Medical Record$")
    fun theGPPracticeHasEnabledAllergiesFunctionalityAndThePatientHasADrugAndNonDrugAllergyRecordGpMedicalRecordEmis() {
        val patient = SerenityHelpers.getPatient()

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisDefaultAllergyModel())
        }
    }

    @Given("the EMIS GP Practice has two allergies results where the first record has no date - GP Medical Record")
    fun givenTheEMISGPPracticeHasTwoAllergiesResultsWhereTheFirstRecordHasNoDateGpMedicalRecord() {
        val patient = SerenityHelpers.getPatient()

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisAllergyRecordsWhereTheFirstRecordHasNoEffectiveDate())
        }
    }

    @Given("the GP Practice has disabled allergies functionality - GP Medical Record")
    fun butTheGPPracticeHasDisabledAllergiesFunctionalityForServiceGpMedicalRecord() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        AllergiesFactory.getForSupplier(getService).disabled(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting allergies for VISION - GP Medical Record$")
    fun thereIsAnUnknownErrorGettingAllergiesForGpMedicalRecord() {
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat )
        { request -> request.respondWithUnknownError() }
    }

    @Then("^I see a message informing me to contact my GP for this information - GP Medical Record$")
    fun thenISeeAMessageInformingMeToContactMyGPGpMedicalRecord() {
        myRecordInfoPage.noRecordsOrNoAccessParagraph.assertIsVisible()
    }

    @Then("^I do not see a message informing me to contact my GP for this information - GP Medical Record$")
    fun thenIDoNotSeeAMessageInformingMeToContactMyGPGpMedicalRecord() {
        myRecordInfoPage.noRecordsOrNoAccessParagraph.assertElementNotPresent()
    }

    @When("^the flag informing that the patient has access to the allergy data is set to \"(.*)\"" +
            " - GP Medical Record$")
    fun andHasAccessToAllergiesDataIsSetToGpMedicalRecord(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the allergy data is set to \"(.*)\"" +
            " - GP Medical Record$")
    fun andHasErrorsWhenRetrievingAllergiesDataIsSetToGpMedicalRecord(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasErrored)
    }

    @Then("^I receive \"(.*)\" allergies as part of the my record object - GP Medical Record$")
    fun thenIReceiveAnAllergiesObjectGpMedicalRecord(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.allergies.data.count())
    }

    @Then("^I see one or more drug type allergies record displayed - GP Medical Record$")
    fun thenISeeOneOrMoreDrugTypeAllergiesRecordDisplayedGpMedicalRecord() {
        val retrievedAllergyRecords = myRecordInfoPage.allergies.allRecordItems().count()
        Assert.assertEquals("Expected allergy record items",
                NUMBER_OF_ALLERGY_RECORDS, retrievedAllergyRecords)
        val expected = MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.getOrFail<ArrayList<String>>()
        val retrievedItemBodies = myRecordInfoPage.allergies.allRecordItemBodies().toTypedArray()

        Assert.assertArrayEquals("Expected allergy record item bodies",
                expected.toArray(), retrievedItemBodies)
    }

    @Then("^I see allergies record displayed - GP Medical Record$")
    fun thenISeeAllergiesRecordDisplayedGpMedicalRecord() {
        val retrievedAllergyRecords = myRecordInfoPage.allergies.allRecordItems().count()
        Assert.assertEquals("Expected allergy record items",
                NUMBER_OF_ALLERGY_RECORDS, retrievedAllergyRecords)
        val expected = MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.getOrFail<ArrayList<String>>()
        val retrievedItemBodies = myRecordInfoPage.allergies.allRecordItemBodies().toTypedArray()
        Assert.assertArrayEquals("Expected allergy record item bodies",
                expected.toArray(), retrievedItemBodies)
    }

    @Then("^I see the expected allergies displayed - GP Medical Record$")
    fun thenISeeTheExpectedAllergiesDisplayedGpMedicalRecord() {
        val getService = SerenityHelpers.getGpSupplier()
        val allergyData = AllergiesFactory
                .getForSupplier(getService)
                .getExpectedAllergies()

        val allergyMessages = allergiesAndReactionsPage.allRecordItems()

        Assert.assertEquals(
                allergyData.count(),
                allergyMessages.count())

        for (i in allergyMessages.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            allergyData[i].date.value
                    ),
                    LocalDate.parse(
                            allergyMessages[i].label,
                            DateTimeFormatter.ofPattern(
                                    DateTimeFormats.frontendBasicDateFormat)
                    )
            )

            // assuming 1 allergy per date for this method until needs expanded
            Assert.assertEquals(allergyData[i].name, allergyMessages[i].bodyElements.joinToString())
        }
    }

    @Then("^I see a drug and non drug allergy record from VISION - GP Medical Record$")
    fun thenISeeADrugAndNonDrugAllergyRecordFromVisionGpMedicalRecord() {
        val allergyMessages = allergiesAndReactionsPage.getAllergiesAndReactionsElements()
        val expectedMessages = listOf(
                "12 May 2007\nH/O: drug allergy\nParacetamol 500mg capsules\nLeg swelling",
                "12 May 2007\nPollen"
        )
        Assert.assertTrue("Expected records", allergyMessages.size == expectedMessages.size )
        allergyMessages.forEachIndexed { i, message -> Assert.assertTrue(message.text == expectedMessages[i]) }
    }

    @Then("^I see the expected allergies displayed with unknown date for the first result - GP Medical Record$")
    fun thenISeeTheExpectedAllergiesDisplayedWithUnknownDateForFirstResultGpMedicalRecord() {

        val expectedAllergies =
                AllergiesData.getEmisAllergyRecordsWhereTheFirstRecordHasNoEffectiveDate().medicalRecord.allergies


        val onScreenAllergies = allergiesAndReactionsPage.allRecordItems()
        Assert.assertEquals(expectedAllergies.size, onScreenAllergies.count())

        for (i in onScreenAllergies.indices) {
            if (i == 0) {
                Assert.assertEquals("Unknown Date", onScreenAllergies[i].label)
            } else {
                val expectedDate = (expectedAllergies[i].effectiveDate.value).takeWhile { !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenAllergies[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                Assert.assertEquals(expectedDate, actualDate)
            }

        }
    }
}