package features.appointments.steps

import features.appointments.factories.AppointmentsCancellingFactory
import features.appointments.factories.UpcomingAppointmentsFactory
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import pages.appointments.CancelAppointmentPage
import pages.navigation.HeaderNative
import worker.WorkerClient
import worker.models.appointments.GenericResponseObject
import java.time.LocalDateTime

open class CancelAppointmentSteps {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps

    lateinit var cancelAppointmentPage: CancelAppointmentPage

    lateinit var headerNative: HeaderNative

    @Step
    fun verifyWeAreOnTheCancelAppointmentScreen() {
        val expectedHeader = "Cancel appointment"
        headerNative.waitForPageHeaderText(expectedHeader)
        assertEquals("Check your appointment details before cancelling",
                cancelAppointmentPage.getCheckDetailsText())
    }

    @Step
    fun verifyTheCorrectAppointmentDetailsAreDisplayed() {
        val expectedSlot = retrieveSlotOfAppointmentToCancel()
        assertEquals("Date", expectedSlot.date, cancelAppointmentPage.selectedAppointmentDate.element.text)
        assertEquals("Time", expectedSlot.time, cancelAppointmentPage.selectedAppointmentTime.element.text)
        assertEquals("Session Name", expectedSlot.session,
                cancelAppointmentPage.selectedAppointmentSessionName.element.text)
        assertEquals("location", expectedSlot.location,
                cancelAppointmentPage.selectedAppointmentLocation.element.text)
        assertEquals(expectedSlot.clinicians, cancelAppointmentPage.getSelectedAppointmentClinicianText())
    }

    @Step
    fun verifyTheDropDownMenuLabel() {
        assertEquals("Reason for cancelling", cancelAppointmentPage.getReasonDropDownLabelText())
    }

    @Step
    fun verifyTheDropDownMenuLabelIsNotVisible() {
        assertFalse("Drop-Down menu is visible", cancelAppointmentPage.containsDropDownMenu())
    }

    @Step
    fun verifyTheValidationErrorSummary() {

        cancelAppointmentPage.validationBanner.assertVisible(arrayListOf("There's a problem",
                "Select a reason for cancelling"))
    }

    @Step
    fun verifyTheInlineReasonValidationError() {
        assertEquals("Select a reason for cancelling", cancelAppointmentPage.reasonError.element.text)
    }

    @Step
    fun selectReason(reason: String) {
        assertTrue("$reason is not available as a Cancellation Reason. ",
                cancelAppointmentPage.selectReason(reason))
    }

    @Step
    private fun retrieveSlotOfAppointmentToCancel(): Slot {
        return Serenity.sessionVariableCalled<List<Slot>>(
                UpcomingAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).first()
    }

    @Step
    fun mockCancellationRequestStubForReason(
            reason: String? = null,
            gpSystem: String,
            response: ((ICancelAppointmentsBuilder) -> Mapping)? = null
    ) {

        val patient = Patient.getDefault(gpSystem)

        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(patient)
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse()

        val factory = AppointmentsCancellingFactory.getForSupplier(gpSystem)
        val request = factory.defaultRequest(
                patient,
                retrieveSlotIdOfAppointmentToCancel(),
                reason
                        ?: Serenity.sessionVariableCalled<MyAppointmentsFacade>(MyAppointmentsFacade::class)
                                .slots!!
                                .cancellationReasons!!
                                .first()
                                .displayName
        )

        factory.setupRequestAndResponse(request, response)
    }

    @Step
    fun retrieveCancellationReasons(): List<GenericResponseObject> {
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .appointments.getMyAppointments(LocalDateTime.now().toString())

        return result.cancellationReasons
    }

    @Step
    fun retrieveSlotIdOfAppointmentToCancel(): Int {
        return retrieveSlotOfAppointmentToCancel().id!!
    }
}
