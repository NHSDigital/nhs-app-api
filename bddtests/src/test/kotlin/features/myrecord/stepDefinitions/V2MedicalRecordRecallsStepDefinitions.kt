package features.myrecord.stepDefinitions

import io.cucumber.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.RecallsPage

open class V2MedicalRecordRecallsStepDefinitions {

    private lateinit var recallsPage: RecallsPage

    val expectedData = arrayOf(
            "1 March 2019\nName 3\nDesc 3\nResult: Result 3\nNext Date: NextDate 3\nStatus: Status 3",
            "1 February 2019\nName 2\nDesc 2\nResult: Result 2\nNext Date: NextDate 2\nStatus: Status 2",
            "Unknown Date\nName 1\nDesc 1\nResult: Result 1\nNext Date: NextDate 1\nStatus: Status 1"
            )

    @Then("^I see the expected recalls - Medical Record v2$")
    fun thenISeeExpectedRecallsV2() {
        val recallMessages = recallsPage.getRecallElements()

        Assert.assertEquals("Expected records", expectedData.size, recallMessages.size )
        recallMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[i], message.text) }
    }
}
