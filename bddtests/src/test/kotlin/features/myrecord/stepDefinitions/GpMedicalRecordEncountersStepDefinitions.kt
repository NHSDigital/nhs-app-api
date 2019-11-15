package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.EncountersPage
import pages.myrecord.MyRecordInfoPage

open class GpMedicalRecordEncountersStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var encountersPage: EncountersPage

    val expectedData = arrayOf(
            "10 July 2019\nSystolic BP Reading 1\nValue: 120\nUnits: No Units Recorded",
            "10 July 2019\nSystolic BP Reading 2\nValue: 120\nUnits: No Units Recorded",
            "10 July 2019\nSystolic BP Reading 3\nValue: 120\nUnits: No Units Recorded")

    @Then("^I see the expected encounters - GP Medical Record$")
    fun thenISeeExpectedEncountersRecordGpMedicalRecord() {
        val encountersMessages = encountersPage.getEncountersElements()

        Assert.assertEquals("Expected records", expectedData.size, encountersMessages.size )
        encountersMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[i], message.text) }
    }

}