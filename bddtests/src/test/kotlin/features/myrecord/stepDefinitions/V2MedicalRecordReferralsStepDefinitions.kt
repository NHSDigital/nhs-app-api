package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.ReferralsPage

open class V2MedicalRecordReferralsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var referralsPage: ReferralsPage

    val expectedData = arrayOf(
        "10 July 2019\nReason: Excision of pilonoidal cyst 1\n" +
                "Speciality: Refer to general surgery special interest GP\nUBRN: None",
        "10 July 2019\nReason: Excision of pilonoidal cyst 2\n" +
                "Speciality: Refer to general surgery special interest GP\nUBRN: None",
        "10 July 2019\nReason: Excision of pilonoidal cyst 3\n" +
                "Speciality: Refer to general surgery special interest GP\nUBRN: None")

    @Then("^I see the expected referrals - Medical Record v2$")
    fun thenISeeExpectedReferralsV2() {
        val referralsElements = referralsPage.getReferralElements()

        Assert.assertEquals("Expected records", expectedData.size, referralsElements.size )
        referralsElements.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[i], message.text) }
    }
}