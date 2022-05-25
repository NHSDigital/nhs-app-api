package features.im1Appointments.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import pages.ErrorDialogPage
import pages.appointments.AvailableAppointmentsPage

class AvailableAppointmentsSlotsErrorStepDefinitions {

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var errorDialogPage: ErrorDialogPage

    @Given("^(.*) user is not allowed to retrieve appointment slots$")
    fun gpSystemUserUsNotAllowedToRetrieveAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @Then("^I see appropriate warning message for loading time-outs$")
    fun iSeeAppropriateWarningMessageForLoadingTimesOutWithPrefixAndUrl() {
        errorDialogPage.assertWarningParagraphText(availableAppointmentsPage.tryLoadingGPAppointmentsAgain)
            .assertWarningParagraphText(availableAppointmentsPage.contactGpSurgeryDirectly)
            .assertWarningParagraphText(availableAppointmentsPage.urgentMedicalAdvice)
            .assertPageHeader(availableAppointmentsPage.cannotShowGpAppointmets)
            .assertPageTitle(availableAppointmentsPage.cannotShowGpAppointmets)
    }

    @Then("^I see appropriate GP warning when there is a loading error with '(.*)' link with a url of '(.*)'$")
    fun iSeeAppropriateGpWarningMessageWhenThereIsALoadingErrorWithPrefix(prefix: String, url: String) {
        errorDialogPage
            .assertPageHeader(availableAppointmentsPage.cannotShowGpAppointments)
            .assertPageTitle(availableAppointmentsPage.cannotShowGpAppointments)
            .assertWarningParagraphText(availableAppointmentsPage.tryLoadingGPAppointmentsAgain)
            .assertWarningParagraphText(availableAppointmentsPage.contactGpSurgeryDirectlyBooking)
            .assertWarningLink(availableAppointmentsPage.contactUsKeepGettingParagraph(prefix).startText, url)
    }

    @Then("^I see appropriate warning when there is a loading error with '(.*)' link with a url of '(.*)'$")
    fun iSeeAppropriateWarningMessageWhenThereIsALoadingErrorWithPrefix(prefix: String, url: String) {
        errorDialogPage
            .assertPageHeader(availableAppointmentsPage.cannotShowAppointments)
            .assertPageTitle(availableAppointmentsPage.cannotShowAppointments)
            .assertWarningParagraphText(availableAppointmentsPage.goBackTryAgain)
            .assertWarningParagraphText(availableAppointmentsPage.bookAppointment)
            .assertWarningLink(availableAppointmentsPage.contactUsKeepGettingParagraph(prefix).startText, url)
    }
}
