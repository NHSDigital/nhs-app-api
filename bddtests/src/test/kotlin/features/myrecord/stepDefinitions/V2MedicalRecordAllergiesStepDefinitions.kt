package features.myrecord.stepDefinitions

import io.cucumber.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.AllergiesAndReactionsPage

open class V2MedicalRecordAllergiesStepDefinitions {

    private lateinit var allergiesAndReactionsPage: AllergiesAndReactionsPage

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

    @Then("^I see the expected allergies displayed with unknown date for the second result - Medical Record v2$")
    fun thenISeeTheExpectedAllergiesDisplayedWithUnknownDateForSecondResultV2() {
        val allergyMessages = allergiesAndReactionsPage.getAllergiesAndReactionsElements()
        val expectedMessages = listOf(
                "17 May 2018\nHay Fever",
                "Unknown Date\nHay Fever"
        )
        Assert.assertEquals("Expected records", expectedMessages.size, allergyMessages.size )
        allergyMessages.forEachIndexed { i, message -> Assert.assertEquals(expectedMessages[i], message.text) }
    }
}
