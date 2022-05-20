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
        errorDialogPage.assertShutterParagraphText(availableAppointmentsPage.tryLoadingGPAppointmentsAgain)
            .assertShutterParagraphText(availableAppointmentsPage.contactGpSurgeryDirectly)
            .assertShutterParagraphText(availableAppointmentsPage.urgentMedicalAdvice)
            .assertPageHeader(availableAppointmentsPage.cannotShowGpAppointmets)
            .assertPageTitle(availableAppointmentsPage.cannotShowGpAppointmets)
    }

    @Then("^I see appropriate warning message when there is a loading error with '(.*)'$")
    fun iSeeAppropriateErrorWarningMessageWhenThereIsALoadingErrorWithPrefix(prefix: String) {
        errorDialogPage.assertWarningParagraphText(availableAppointmentsPage.getGoBackAndTryAgainParagraph(prefix))
                .assertWarningParagraphText(availableAppointmentsPage.ifItContinues)
                .assertPageHeader(availableAppointmentsPage.problemLoadingTitle)
                .assertPageTitle(availableAppointmentsPage.problemLoadingTitle)
    }
}
