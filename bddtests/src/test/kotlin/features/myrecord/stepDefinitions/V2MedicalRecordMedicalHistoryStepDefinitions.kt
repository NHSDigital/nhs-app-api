package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.MedicalHistoryPage

open class V2MedicalRecordMedicalHistoryStepDefinitions {

    private lateinit var medicalHistoryPage: MedicalHistoryPage

    private val expectedData = arrayOf(
        "3 July 2019\nRubric\nDescription",
        "3 July 2019\nRubric\nDescription",
        "3 July 2019\nRubric\nDescription")

    @Then("^I see the expected medical history - Medical Record v2$")
    fun thenISeeExpectedMedicalHistoryV2() {
        val medicalHistoryItems = medicalHistoryPage.getMedicalHistoryElements()

        Assert.assertEquals(expectedData.size, medicalHistoryItems.size)

        medicalHistoryItems.forEachIndexed { i, item ->
            Assert.assertEquals(expectedData[i], item.text) }
    }
}