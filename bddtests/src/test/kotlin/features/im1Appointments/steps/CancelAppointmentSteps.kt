package features.im1Appointments.steps

import features.im1Appointments.factories.AppointmentsCancellingFactory
import mocking.stubs.appointments.factories.MyAppointmentsFactory
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
import pages.navigation.WebHeader
import pages.text
import worker.WorkerClient
import worker.models.appointments.GenericResponseObject
import java.time.LocalDateTime

open class CancelAppointmentSteps {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    lateinit var webHeader: WebHeader

    private lateinit var cancelAppointmentPage: CancelAppointmentPage

    @Step
    fun verifyWeAreOnTheCancelAppointmentScreen() {
        cancelAppointmentPage.assertPageFullyLoaded()
        assertEquals("Check your appointment details before cancelling",
                cancelAppointmentPage.getCheckDetailsText())
    }

    @Step
    fun verifyTheCorrectAppointmentDetailsAreDisplayed() {
        val expectedSlot = retrieveSlotOfAppointmentToCancel().copy(id = null)
        val areCliniciansExpected = expectedSlot.clinicians.isNotEmpty()
        val actualSlot = cancelAppointmentPage.getAppointmentSlot(areCliniciansExpected)
        assertEquals("Exact expected Appointment not found. ", expectedSlot, actualSlot)
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
        assertEquals("Select a reason for cancelling", cancelAppointmentPage.reasonError.text)
    }

    @Step
    fun selectReason(reason: String) {
        assertTrue("$reason is not available as a Cancellation Reason. ",
                cancelAppointmentPage.selectReason(reason))
    }

    @Step
    private fun retrieveSlotOfAppointmentToCancel(): Slot {
        return Serenity.sessionVariableCalled<List<Slot>>(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).first()
    }

    @Step
    fun mockCancellationRequestStubForReason(
            reason: String? = null,
            gpSystem: String,
            response: ((ICancelAppointmentsBuilder) -> Mapping)? = null
    ) {

        val patient = Patient.getDefault(gpSystem)

        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(patient)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse()

        val factory = AppointmentsCancellingFactory.getForSupplier(gpSystem)
        val request = factory.defaultRequest(
                patient,
                retrieveSlotIdOfAppointmentToCancel(),
                reason
                        ?: Serenity.sessionVariableCalled<MyAppointmentsFacade>(MyAppointmentsFacade::class)
                                .myAppointments!!
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

    @Step
    fun clickOnBreadcrumb() {
        webHeader.getBreadCrumbToGoBackOneLevel().click()
    }
}
