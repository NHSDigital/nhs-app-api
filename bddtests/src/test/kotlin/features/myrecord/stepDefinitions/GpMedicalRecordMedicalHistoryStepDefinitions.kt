package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.MedicalHistoryPage
import pages.myrecord.MyRecordInfoPage

open class GpMedicalRecordMedicalHistoryStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    private lateinit var medicalHistoryPage: MedicalHistoryPage

    private val expectedData = arrayOf(
        "3 July 2019\nRubric\nDescription",
        "3 July 2019\nRubric\nDescription",
        "3 July 2019\nRubric\nDescription")

    @Then("^I see the expected medical history - GP Medical Record$")
    fun thenISeeExpectedMedicalHistoryGpMedicalRecord() {
        val medicalHistoryItems = medicalHistoryPage.getMedicalHistoryElements()

        Assert.assertEquals(medicalHistoryItems.size, expectedData.size )

        medicalHistoryItems.forEachIndexed { i, item ->
            Assert.assertEquals(item.text, expectedData.get(i)) }
    }
}