package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.TppDcrData
import mocking.defaults.EmisMockDefaults
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import pages.gpMedicalRecord.ConsultationsPage
import pages.gpMedicalRecord.EventsPage
import utils.SerenityHelpers
import java.lang.UnsupportedOperationException

open class GpMedicalRecordConsultationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var consultationsPage: ConsultationsPage
    lateinit var eventsPage: EventsPage

    val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                    "18 February 2018\nTHE SURGERY - MOSS - Jean (Dr)",
                    "18 February 2018\nTHE SURGERY - MOSS - Jean (Dr)"
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

    @Given("^the GP Practice has multiple consultations - GP Medical Record$")
    fun givenTheGpPracticeHasMultipleConsultationsFor() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis)
                            .respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(SerenityHelpers.getPatient().tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
                }
            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }

    }

    @Then("^I see the expected consultations and events - GP Medical Record$")
    fun thenISeeExpectedConsultationsGpMedicalRecord() {
        val supplier = SerenityHelpers.getGpSupplier()
        val consultationsAndEventsMessages = getConsultationsOrEventsData(supplier)

        Assert.assertTrue(
                "Expected records", consultationsAndEventsMessages!!.size == expectedData[supplier]?.size )
        consultationsAndEventsMessages.forEachIndexed { i, message -> run {
            val msg = message.text
            val expected = expectedData[supplier]?.get(i)
            print(msg)
            print(expected)
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text)
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