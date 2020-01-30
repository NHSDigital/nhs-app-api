package features.im1Appointments.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import pages.ErrorDialogPage
import pages.appointments.AvailableAppointmentsPage

class AvailableAppointmentsSlotsErrorStepDefinitions {

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var errorDialogPage: ErrorDialogPage

    val mockingClient = MockingClient.instance

    @Given("^(.*) user is not allowed to retrieve appointment slots$")
    fun gpSystemUserUsNotAllowedToRetrieveAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @Then("^I see appropriate error message for loading time-outs with '(.*)'$")
    fun iSeeAppropriateErrorMessageForLoadingTimesOutWithPrefix(prefix: String) {
        errorDialogPage.assertParagraphText(availableAppointmentsPage.getTryAgainNowParagraph(prefix))
                .assertParagraphText(availableAppointmentsPage.ifItContinues)
                .assertPageHeader(availableAppointmentsPage.problemLoadingTitle)
                .assertPageTitle(availableAppointmentsPage.problemLoadingTitle)
    }

    @Then("^I see appropriate error message when there is a loading error with '(.*)'$")
    fun iSeeAppropriateErrorMessageWhenThereIsALoadingErrorWithPrefix(prefix: String) {
        errorDialogPage.assertParagraphText(availableAppointmentsPage.getGoBackAndTryAgainParagraph(prefix))
                .assertParagraphText(availableAppointmentsPage.ifItContinues)
                .assertPageHeader(availableAppointmentsPage.problemLoadingTitle)
                .assertPageTitle(availableAppointmentsPage.problemLoadingTitle)
    }
}
