package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Then
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import pages.gpMedicalRecord.ConsultationsPage
import pages.gpMedicalRecord.EventsPage
import utils.SerenityHelpers

open class V2MedicalRecordConsultationsStepDefinitions {

    private lateinit var consultationsPage: ConsultationsPage
    private lateinit var eventsPage: EventsPage

    val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                    "18 February 2018\nTHE SURGERY - MOSS - Jean (Dr)",
                    "17 February 2018\nTHE SURGERY - MOSS - Jean (Dr)"
            ), Supplier.TPP to arrayOf(
            "16 February 2018\n" +
                "Kainos GP Demo Unit (General Practice) - Mr General NhsApp\n" +
                "Medication Template - Alimemazine 50mg tablets - 1 pack of 14 tablet(s)" +
                " - [08:00-1][12:00-1][16:00-1][22:00-1]\n" +
                "Medication - (R) Benzoin tincture - 250 ml - use as directed",
            "15 March 2014\n" +
                    "Kainos GP Demo Unit (General Practice) - Mr General NhsApp\n" +
                    "Medication Template - Alimemazine 10mg tablets - 1 pack of 28 tablet(s)" +
                    " - [08:00-1][12:00-1][16:00-1][22:00-1]\n" +
                    "Medication - Benzoin tincture - 500 ml - use as directed"
            ))

    @Then("^I see the expected consultations and events - Medical Record v2$")
    fun thenISeeExpectedConsultationsV2() {
        val supplier = SerenityHelpers.getGpSupplier()
        val consultationsAndEventsMessages = getConsultationsOrEventsData(supplier)

        Assert.assertEquals(
                "Expected records", expectedData[supplier]?.size, consultationsAndEventsMessages!!.size)

        consultationsAndEventsMessages.forEachIndexed { i, message -> run {
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text)
        }}
    }

    @Then("^I see the second consultation record have unknown date - Medical Record v2$")
    fun thenISeeTheSecondExpectedConsultationRecordHaveUnknownDateV2() {
        val supplier = SerenityHelpers.getGpSupplier()
        val consultationsAndEventsMessages = getConsultationsOrEventsData(supplier)

        val dataWithUnknownDate = expectedData[supplier]
        dataWithUnknownDate?.set(1, dataWithUnknownDate[1].replace("17 February 2018", "Unknown Date"))

        Assert.assertTrue(
                "Expected records", consultationsAndEventsMessages!!.size == dataWithUnknownDate?.size )
        consultationsAndEventsMessages.forEachIndexed { i, message -> run {
            Assert.assertEquals(dataWithUnknownDate?.get(i), message.text)
        }}
    }

    private fun getConsultationsOrEventsData(supplier: Supplier): List<WebElementFacade>? {
        var elements: List<WebElementFacade>? = null
        if(supplier == Supplier.EMIS) {
            elements = consultationsPage.getConsultationsElements()
        } else if(supplier == Supplier.TPP) {
            elements = eventsPage.getEventsElements()
        }
        return elements
    }
}
