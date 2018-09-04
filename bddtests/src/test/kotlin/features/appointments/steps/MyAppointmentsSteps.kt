package features.appointments.steps

import com.google.common.collect.Ordering
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import features.appointments.factories.ViewAppointmentsFactory
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import features.sharedSteps.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.MockDefaults
import models.Patient
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.appointments.MyAppointmentsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.util.*

open class MyAppointmentsSteps {
    @Steps
    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    lateinit var myAppointmentsPage: MyAppointmentsPage

    val pageHeader = "My appointments"
    val expectedNoUpcomingText = "You don't currently have any appointments booked\n" +
            "Once you've booked an appointment here, you'll be able to view details, cancel it and see your appointment history.\n" +
            "If you have an upcoming appointment that isn't shown here, contact your GP surgery for more information."
    val bookingSuccessMessage = "Your appointment has been booked. You can view details or cancel it here."
    val cancellationSuccessMessage = "Your appointment has been cancelled."

    @Step
    fun checkBookingSuccessMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(bookingSuccessMessage, message)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        myAppointmentsPage.bookButton.element.click()
    }

    @Step
    fun waitForSpinnerToDisappear() {
        myAppointmentsPage.waitForSpinnerToDisappear()
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        assertTrue("Expected Header text is not found: $pageHeader",
                myAppointmentsPage.waitForPageHeaderText(pageHeader))
    }

    @Step
    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated() {
        val serviceFactory = getActiveAppointmentsFactory()
        val expectedSlots = serviceFactory.getAppointmentData().generateExpectedMyAppointments()
        val areCliniciansExpected = expectedSlots.isNotEmpty() || expectedSlots[0].clinician.isNotEmpty()
        val slots = myAppointmentsPage.getAllSlots(areCliniciansExpected)
        assertEquals("Expected upcoming myAppointments size doesn't match with the actual size",
                expectedSlots.size, slots.size)
        assertEquals("Exact expected Appointments list not found. ", expectedSlots, slots)
    }

    @Step
    fun checkIfSlotsAreInCorrectOrder(): Boolean {
        val slotDate = myAppointmentsPage.getDateTimestampsOfSlots()
        return Ordering.natural<Long>().isOrdered(slotDate)
    }

    @Step
    fun checkNoUpcomingAppointmentsTextIsDisplaying() {
        val actualNoUpcomingText = myAppointmentsPage.getNoUpcomingText()
        assertEquals("Incorrect text when no upcoming appointments. ",
                expectedNoUpcomingText, actualNoUpcomingText)
    }

    @Step
    fun checkIfBookAnAppointmentButtonExistAndEnabled() {
        try {
            myAppointmentsPage.bookButton.element.isVisible
            assertTrue("Book an appointment is not displaying",
                    myAppointmentsPage.bookButton.element.isDisplayed)

            assertTrue("Book an appointment is not enabled",
                    myAppointmentsPage.bookButton.element.isCurrentlyEnabled)
        } catch (e: Exception) {
            fail("Book an appointment is not found")
        }
    }


    @Step
    fun mockGPServiceMyAppointmentResponse(gpService: String, noUpcomingAppointments: Boolean = false) {
        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(gpService)
        val patient = viewAppointmentFactory.getAppointmentData().defaultPatient
        Serenity.setSessionVariable(Patient::class).to(patient)

        val getResponse = when {
            noUpcomingAppointments -> viewAppointmentFactory.createEmptyUpcomingAppointmentResponse(patient)
            else -> viewAppointmentFactory.createUpcomingAppointments(patient)
        }
        viewAppointmentFactory.setUpViewAppointmentsWithResult(gpService) { builder ->
            builder.respondWithSuccess(getResponse)
        }
    }

    @Step
    fun generateStubsForMyAppointmentsWhenUnavailableToPatient(provider: String) {
        val patient = SerenityHelpers.getPatient()
        val currentViewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.setupViewAppointmentResponse {
            viewMyAppointmentsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    @Step
    fun createSerenityMyAppointmentSessionVariable() {
        val timeZone = TimeZone.getTimeZone("Europe/London")
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        dateTimeFormat.timeZone = timeZone
        val fromDate = dateTimeFormat.format(Calendar.getInstance().time)
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getMyAppointments(fromDate)
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Step
    fun createSerenityEmisMyAppointmentSessionVariable() {
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val fromDate = dateTimeFormat.format(Calendar.getInstance().time)
        val result = Serenity
                .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .getMyAppointments(fromDate)
        Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
    }

    @Step
    fun setCsrfToken(crsfToken: String) {
        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .setCsrfToken(crsfToken)
    }

    @Step
    fun checkMyAppointmentsAreAllUpcomingOnes() {
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(MyAppointmentsResponse::class.java)
        val now = Date().time
        myAppointmentsResponse.appointments.forEach { appointment ->
            val startTime = dateTimeFormat.parse(appointment.startTime).time
            assertTrue("appointment with slot id ${appointment.id} is a past appointment",
                    now <= startTime)
        }
    }

    @Step
    fun checkCancellationReasonExistForApplicableGPService() {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(MyAppointmentsResponse::class.java)
        val cancellationReasons = myAppointmentsResponse.cancellationReasons

        val expectedCancellationReasons = getActiveAppointmentsFactory().getAppointmentData().getAppointmentCancellationReasons()
        assertTrue("EMIS cancellation options count doesn't match",
                cancellationReasons.size == expectedCancellationReasons?.size ?: 0)
        expectedCancellationReasons?.forEach { expectedReason ->
            val actualReason = cancellationReasons.firstOrNull { expectedReason.displayName == it.displayName }
            assertNotNull("Expected reason ${expectedReason.displayName} not found",
                    actualReason)
        }
    }

    @Step
    fun clickFirstCancelLink() {
        myAppointmentsPage.clickFirstCancelAppointmentLink()
    }

    @Step
    fun storeDetailsOfFirstAppointment() {
        Serenity.setSessionVariable(Slot::class.java).to(myAppointmentsPage.getSlotAtIndex(0))
    }

    @Step
    fun verifyCancellationConfirmationMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(cancellationSuccessMessage, message)
    }

    @Step
    fun verifyThatThereIsACancelLinkForEachUpcomingAppointment() {
        assertEquals("Missing at least one cancel link. ", myAppointmentsPage.getWebAppointmentSlotDivs().size, myAppointmentsPage.getNumberOfCancelLinks())
    }

    private fun getActiveAppointmentsFactory(gpService: String? = null): ViewAppointmentsFactory {
        val thGpService = gpService ?: Serenity.sessionVariableCalled<String>(GLOBAL_PROVIDER_TYPE)
        return ViewAppointmentsFactory.getForSupplier(thGpService)
    }
}
