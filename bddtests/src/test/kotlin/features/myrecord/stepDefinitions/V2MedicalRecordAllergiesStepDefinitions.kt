package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.AllergiesFactory
import mocking.data.myrecord.AllergiesData
import org.junit.Assert
import pages.gpMedicalRecord.AllergiesAndReactionsPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

open class V2MedicalRecordAllergiesStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var allergiesAndReactionsPage: AllergiesAndReactionsPage

    @Then("^I see the expected allergies displayed - Medical Record v2$")
    fun thenISeeTheExpectedAllergiesDisplayedV2() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val allergyData = AllergiesFactory
                .getForSupplier(gpSystem)
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

    @Then("^I see a drug and non drug allergy record from VISION - Medical Record v2$")
    fun thenISeeADrugAndNonDrugAllergyRecordFromVisionV2() {
        val allergyMessages = allergiesAndReactionsPage.getAllergiesAndReactionsElements()
        val expectedMessages = listOf(
                "12 May 2007\nH/O: drug allergy\nParacetamol 500mg capsules\nLeg swelling",
                "12 May 2007\nPollen"
        )
        Assert.assertEquals("Expected records", expectedMessages.size, allergyMessages.size )
        allergyMessages.forEachIndexed { i, message -> Assert.assertEquals(expectedMessages[i], message.text) }
    }

    @Then("^I see the expected allergies displayed with unknown date for the first result - Medical Record v2$")
    fun thenISeeTheExpectedAllergiesDisplayedWithUnknownDateForFirstResultV2() {
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