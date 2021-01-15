package features.im1Appointments.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.im1Appointments.steps.AvailableAppointmentsSteps
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import pages.NoOnlineAppointments

class AvailableAppointmentsSlotsGuidanceStepDefinitions {

    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    @Steps
    lateinit var noOnlineAppointmentsMessage: NoOnlineAppointments

    @Given("^there are available appointment slots with different criteria for (.*) " +
            "when (.*) appointment slot guidance is provided$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaWithCustomGuidance(
            gpSystem: String, guidanceMessageDescription: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)

        val guidanceMessage = when (guidanceMessageDescription) {
            "empty" -> ""
            "whitespace string" -> "   "
            else -> guidanceMessageDescription
        }
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample(guidanceMessage = guidanceMessage)
    }

    @When("^I acknowledge that there are no appointments and go back to my appointments$")
    fun iAcknowledgeThatThereAreNoAppointmentsAndGoBackToMyAppointments() {
        noOnlineAppointmentsMessage.clickBackLink()
    }

    @Then("^I see a message informing me that the GP has no online appointments available and what to do next")
    fun iSeeANoOnlineAppointmentsMessage() {
        noOnlineAppointmentsMessage.assertPageHeader("No appointments available to book online at this time")
        noOnlineAppointmentsMessage.assertParagraphText("You\'ll need to contact your GP surgery " +
                "to book an appointment.")
        noOnlineAppointmentsMessage.assertParagraphText("For urgent medical advice, go to")
        noOnlineAppointmentsMessage.assertLinkExists(
                "Use the 111 coronavirus service to see if you need medical help.",
                "https://111.nhs.uk/COVID-19", false)
        noOnlineAppointmentsMessage.assertCoronaVirusInfoHeader()
                .assertParagraphText("Stay at home and avoid close contact with other people.")
                .assertGpAdminMenuItem()
                .assertGpAdviceMenuItem()
                .assertNHS111Online()
    }

    @Given("^there are available appointment slots with different criteria for (.*) when guidance cannot be retrieved$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForEmisWhenGuidanceCannotBeRetrieved(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage()
    }

    @When("^I expand the appointment slot guidance$")
    fun iExpandTheAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.assertLabel("Which type of appointment do I need?")
        availableAppointments.availableAppointmentsPage.guidance.expand()
    }

    @Then("^the appointment slot guidance content is displayed$")
    fun appointmentSlotGuidanceContentIsDisplayed() {
        val expectedGuidanceContent =
                sessionVariableCalled<String>(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)

        availableAppointments.availableAppointmentsPage.guidance.assertContent(expectedGuidanceContent)
    }

    @Then("^the appointment slot guidance is collapsible$")
    fun appointmentSlotGuidanceIsCollapsible() {
        availableAppointments.availableAppointmentsPage.guidance.collapse()
        iExpandTheAppointmentSlotGuidance()
    }

    @Then("^I cannot see any appointment slot guidance$")
    fun iCannotSeeAnyAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.assertNotPresent()
    }
}
