package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.AllergiesFactory
import features.myrecord.factories.MyRecordVisionMocker
import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.allergiesView
import net.serenitybdd.core.Serenity
import org.junit.Assert
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.myrecord.MyRecordInfoPage
import mocking.data.myrecord.NUMBER_OF_ALLERGY_RECORDS
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.myrecord.MyRecordResponse
import java.lang.UnsupportedOperationException
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_ALLERGIES = 5

open class MyRecordAllergiesStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergies(count: Int) {
        val gpSystem = SerenityHelpers.getGpSupplier()
        AllergiesFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient(), count)
    }

    @Given("^the GP Practice has enabled allergies functionality and has a drug and non drug allergy " +
            "record for VISION$")
    fun theGPPracticeHasEnabledAllergiesFunctionalityAndThePatientHasADrugAndNonDrugAllergyRecord() {
        val gpSystem = Supplier.VISION
        MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(gpSystem.supplierName)

        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesDrugAndNonDrugData()) }
    }

    @Given("the GP Practice has enabled allergies functionality and has 5 different " +
            "allergies with different date formats")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndHasFiveDifferentAllergiesWithDifferentDateFormats() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(gpSystem.supplierName)

        when (gpSystem) {
            Supplier.EMIS ->
                mockingClient.forEmis {
                    myRecord.allergiesRequest(patient)
                            .respondWithSuccess(AllergiesData.getEmisAllergyRecordsWithDifferentDateParts())
                }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithSuccess(AllergiesData.getTppAllergiesData(NUMBER_OF_ALLERGIES))
                }
            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }
    }

    @Given("^the EMIS GP Practice has two allergies results where the first record has no date$")
    fun givenTheEMISGPPracticeHasTwoAllergiesResultsWhereTheFirstRecordHasNoDate() {
        val patient = MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(Supplier.EMIS.supplierName)

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisAllergyRecordsWhereTheFirstRecordHasNoEffectiveDate())
        }
    }

    @Given("the GP Practice has disabled allergies functionality")
    fun butTheGPPracticeHasDisabledAllergiesFunctionalityForService() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(gpSystem.supplierName)

        AllergiesFactory.getForSupplier(gpSystem).disabled(patient)
    }

    @Given("^there is an unknown error getting allergies for VISION$")
    fun thereIsAnUnknownErrorGettingAllergiesFor() {
        val gpSystem = Supplier.VISION
        val patient = MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(gpSystem.supplierName)

        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                patient,
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
        val retrievedAllergyRecords = myRecordInfoPage.allergies.allRecordItems().count()
        Assert.assertEquals("Expected allergy record items",
                NUMBER_OF_ALLERGY_RECORDS, retrievedAllergyRecords)
        val expected = MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.getOrFail<ArrayList<String>>()
        val retrievedItemBodies = myRecordInfoPage.allergies.allRecordItemBodies().toTypedArray()

        Assert.assertArrayEquals("Expected allergy record item bodies",
                expected.toArray(), retrievedItemBodies)
    }

    @Then("^I see allergies record displayed$")
    fun thenISeeAllergiesRecordDisplayed() {
        val retrievedAllergyRecords = myRecordInfoPage.allergies.allRecordItems().count()
        Assert.assertEquals("Expected allergy record items",
                NUMBER_OF_ALLERGY_RECORDS, retrievedAllergyRecords)
        val expected = MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.getOrFail<ArrayList<String>>()
        val retrievedItemBodies = myRecordInfoPage.allergies.allRecordItemBodies().toTypedArray()
        Assert.assertArrayEquals("Expected allergy record item bodies",
                expected.toArray(), retrievedItemBodies)
    }

    @Then("^I see the expected allergies displayed$")
    fun thenISeeTheExpectedAllergiesDisplayed() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val allergyData = AllergiesFactory
                .getForSupplier(gpSystem)
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
        assertContains(dates, "15 May 2018 9:52 am")
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

    @Then("^I see the expected allergies displayed with unknown date for the first result$")
    fun thenISeeTheExpectedAllergiesDisplayedWithUnknownDateForFirstResult() {

        val expectedAllergies =
                AllergiesData.getEmisAllergyRecordsWhereTheFirstRecordHasNoEffectiveDate().medicalRecord.allergies


        val onScreenAllergies = myRecordInfoPage.allergies.allRecordItems()
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