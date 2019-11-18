package features.im1Appointments.steps

import constants.DateTimeFormats.Companion.backendDateTimeFormatWithoutTimezone
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import mocking.MockingClient
import mocking.emis.models.AppointmentCancellationReason
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.util.*

open class YourAppointmentsBackendSteps {

    val mockingClient = MockingClient.instance

    @Step
    fun createSerenityMyAppointmentSessionVariable() {
        val timeZone = TimeZone.getTimeZone("Europe/London")
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        dateTimeFormat.timeZone = timeZone
        val fromDate = dateTimeFormat.format(Calendar.getInstance().time)
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.getMyAppointments(fromDate)
            println(result)
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
            println(httpException)
        }
    }

    @Step
    fun checkUpcomingAppointments(presence: Boolean = true) {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val expectedUpcomingAppointments = if (presence) {
            Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                    MyAppointmentsFactory.Expectations.EXPECTED_API_RESPONSE_OF_MY_APPOINTMENTS
            ).upcomingAppointments
        } else {
            arrayListOf()
        }
        assertEquals(
                "Incorrect number of appointments returned. ",
                expectedUpcomingAppointments.count(),
                myAppointmentsResponse.upcomingAppointments.size
        )
        assertArrayEquals(
                "List of appointments returned is incorrect. ",
                expectedUpcomingAppointments.toTypedArray(),
                myAppointmentsResponse.upcomingAppointments.toTypedArray()
        )
    }

    @Step
    fun checkHistoricalAppointments(presence: Boolean = true) {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val expectedHistoricalAppointments = if (presence) {
            Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                    MyAppointmentsFactory.Expectations.EXPECTED_API_RESPONSE_OF_MY_APPOINTMENTS
            ).pastAppointments
        } else {
            arrayListOf()
        }
        assertEquals(
                "Incorrect number of appointments returned. ",
                expectedHistoricalAppointments.count(),
                myAppointmentsResponse.pastAppointments.size
        )
        assertArrayEquals(
                "List of appointments returned is incorrect. ",
                expectedHistoricalAppointments.toTypedArray(),
                myAppointmentsResponse.pastAppointments.toTypedArray()
        )
    }

    @Step
    fun checkCancellationReasonExistForApplicableGPService() {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val cancellationReasons = myAppointmentsResponse.cancellationReasons

        val expectedCancellationReasons = Serenity.sessionVariableCalled<List<AppointmentCancellationReason>>(
                AppointmentCancellationReason::class
        )
        assertEquals("Cancellation options count doesn't match",
                expectedCancellationReasons?.size ?: 0,
                cancellationReasons.size
        )
        expectedCancellationReasons?.forEach { expectedReason ->
            val actualReason = cancellationReasons.firstOrNull { expectedReason.displayName == it.displayName }
            assertNotNull("Expected reason ${expectedReason.displayName} not found",
                    actualReason)
        }
    }
}
