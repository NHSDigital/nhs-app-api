package features.im1Appointments.steps

import mocking.stubs.appointments.factories.MyAppointmentsFactory
import mocking.MockingClient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ErrorPage
import pages.appointments.MyAppointmentsPage
import pages.isDisplayed
import pages.navigation.HeaderNative

open class MyAppointmentsTelephoneSteps {
    val mockingClient = MockingClient.instance

    lateinit var myAppointmentsPage: MyAppointmentsPage
    @Steps
    lateinit var myAppointmentsUiSteps: MyAppointmentsUISteps
    lateinit var errorPage: ErrorPage
    lateinit var headerNative: HeaderNative

    private val upcomingPhoneMessage = "We will call you on"
    private val pastPhoneMessage = "The number you gave us was"

    @Step
    fun checkUpcomingTelephoneAppointmentsAreCorectlyPopulated() {
        myAppointmentsPage.upcomingAppointmentsHeading.isDisplayed
        myAppointmentsUiSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS,
                true
        )
        checkAppointmentsTelephoneAppointmentsTextCorrect(true)
    }

    @Step
    fun checkPastTelephoneAppointmentsAreCorectlyPopulated() {
        myAppointmentsPage.pastAppointmentsHeading.isDisplayed
        myAppointmentsUiSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS,
                true
        )
        checkAppointmentsTelephoneAppointmentsTextCorrect(false)
    }

    private fun checkAppointmentsTelephoneAppointmentsTextCorrect(
            isUpcomingAppointment: Boolean
    ) {
        val phoneFields = myAppointmentsPage.getTelephoneField()
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