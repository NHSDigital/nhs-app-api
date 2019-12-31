package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.EncountersPage

open class V2MedicalRecordEncountersStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var encountersPage: EncountersPage

    val expectedData = arrayOf(
            "10 July 2019\nSystolic BP Reading 1\nValue: 120\nUnits: No Units Recorded",
            "10 July 2019\nSystolic BP Reading 2\nValue: 120\nUnits: No Units Recorded",
            "10 July 2019\nSystolic BP Reading 3\nValue: 120\nUnits: No Units Recorded")

    @Then("^I see the expected encounters - Medical Record v2$")
    fun thenISeeExpectedEncountersV2() {
        val encountersMessages = encountersPage.getEncountersElements()

        Assert.assertEquals("Expected records", expectedData.size, encountersMessages.size )
        encountersMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[i], message.text) }
    }

}