package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.ImmunisationsPage
import utils.SerenityHelpers

open class V2MedicalRecordImmunisationsStepDefinitions {

    private lateinit var immunisationsPage: ImmunisationsPage

    val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                "18 February 2018\nSecond meningitis C Vaccination",
                "15 May 2002\nFirst meningitis C Vaccination"
            ), Supplier.VISION to arrayOf(
                "10 October 2018\nLumpectomy NEC",
                "10 October 2018\nLumpectomy NEC"
            ), Supplier.MICROTEST to arrayOf(
                "3 July 2019\nImmunisation 1\nNext Date: no next date\nStatus: Main 1",
                "3 July 2019\nImmunisation 2\nNext Date: no next date\nStatus: Main 2",
                "3 July 2019\nImmunisation 3\nNext Date: no next date\nStatus: Main 3"
            ))

    @Then("^I see the expected immunisations - Medical Record v2$")
    fun thenISeeExpectedImmunisationsV2() {
        val immunisationsMessages = immunisationsPage.getImmunisationsElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertEquals(
                "Expected records", expectedData[supplier]?.size, immunisationsMessages.size )
        immunisationsMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text) }
    }

    @Then("^I see the expected immunisations with an unknown date for the second result - Medical Record v2$")
    fun thenISeeExpectedImmunisationsWithUnknownDateV2() {
        val immunisationsMessages = immunisationsPage.getImmunisationsElements()

        val expectedMessages = listOf(
            "18 February 2018\nSecond meningitis C Vaccination",
            "Unknown Date\nFirst meningitis C Vaccination"
        )

        Assert.assertEquals("Expected records", expectedMessages.size, immunisationsMessages.size )
        immunisationsMessages.forEachIndexed { i, message -> Assert.assertEquals(expectedMessages[i], message.text) }
    }
}
