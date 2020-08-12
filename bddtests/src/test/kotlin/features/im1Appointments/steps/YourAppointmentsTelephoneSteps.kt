package features.im1Appointments.steps

import mocking.stubs.appointments.factories.MyAppointmentsFactory
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.appointments.YourAppointmentsPage
import pages.assertIsDisplayed
import pages.navigation.HeaderNative

open class YourAppointmentsTelephoneSteps {

    lateinit var yourAppointmentsPage: YourAppointmentsPage
    @Steps
    lateinit var yourAppointmentsUiSteps: YourAppointmentsUISteps
    lateinit var headerNative: HeaderNative

    private val upcomingPhoneMessage = "We will call you on"
    private val pastPhoneMessage = "The number you gave us was"

    @Step
    fun checkUpcomingTelephoneAppointmentsAreCorectlyPopulated() {
        yourAppointmentsPage.upcomingAppointmentsHeading
                .assertIsDisplayed("Expected Upcoming Appointment Heading")
        yourAppointmentsUiSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS,
                true
        )
        checkAppointmentsTelephoneAppointmentsTextCorrect(true)
    }

    @Step
    fun checkPastTelephoneAppointmentsAreCorectlyPopulated() {
        yourAppointmentsPage.pastAppointmentsHeading
                .assertIsDisplayed("Expected Telephone Appointment Heading")
        yourAppointmentsUiSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS,
                true
        )
        checkAppointmentsTelephoneAppointmentsTextCorrect(false)
    }

    private fun checkAppointmentsTelephoneAppointmentsTextCorrect(
            isUpcomingAppointment: Boolean
    ) {
        val phoneFields = yourAppointmentsPage.getTelephoneField()
        Assert.assertNotNull("Invalid session variable key. ", phoneFields)
        phoneFields.forEach { string ->
            val compareString = string.split(" ").dropLast(1).joinToString(separator = " ")
            if (isUpcomingAppointment)
                Assert.assertTrue("Upcoming telephone message doesn't match",
                        upcomingPhoneMessage.startsWith(compareString))
            else
                Assert.assertEquals("Past telephone message doesn't match", pastPhoneMessage, compareString)
        }
    }
}
