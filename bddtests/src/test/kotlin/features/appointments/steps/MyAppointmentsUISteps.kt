package features.appointments.steps

import com.google.common.collect.Ordering
import features.appointments.factories.MyAppointmentsFactory
import mocking.MockingClient
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import pages.ErrorPage
import pages.appointments.MyAppointmentsPage
import pages.navigation.HeaderNative
import java.util.*

open class MyAppointmentsUISteps {

    val mockingClient = MockingClient.instance

    lateinit var myAppointmentsPage: MyAppointmentsPage
    lateinit var errorPage: ErrorPage
    lateinit var headerNative: HeaderNative

    private val pageHeader = "My appointments"

    private val bookingSuccessMessage = "Your appointment has been booked. You can view details or cancel it here."
    private val cancellationSuccessMessage = "Your appointment has been cancelled."


    private val expectedNoUpcomingText = "Upcoming appointments\n" +
            "You don't currently have any appointments booked.\n" +
            "Once you've booked an appointment here, you'll be able to view details and cancel it.\n" +
            "If you have an upcoming appointment that isn't shown here, contact your GP surgery for more information."
    private val expectedNoPastText = "Past appointments\n" +
            "You have no recent past appointments. To find out about older appointments, contact your GP surgery."

    @Step
    fun checkBookingSuccessMessage(includesReferenceToCancel: Boolean = true) {
        val actualMessage = myAppointmentsPage.getSuccessMessage()
        val expectedMessage =
                if (includesReferenceToCancel)
                    bookingSuccessMessage
                else
                    bookingSuccessMessage.replace("or cancel it ", "")
        assertEquals(expectedMessage, actualMessage)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        myAppointmentsPage.locatorMethods.assertNativeElementsLoaded(myAppointmentsPage.bookButton)
        myAppointmentsPage.bookButton.click()
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        headerNative.waitForPageHeaderText(pageHeader)
    }

    @Step
    fun checkNoUpcomingAppointmentsTextIsDisplaying() {
        val actualNoUpcomingText = myAppointmentsPage.getNoUpcomingText()
        Assert.assertEquals("Incorrect text when no upcoming appointments. ",
                expectedNoUpcomingText, actualNoUpcomingText)
    }

    @Step
    fun checkNoHistoricalAppointmentsTextIsDisplaying() {
        val actualNoPastText = myAppointmentsPage.getNoPastText()
        Assert.assertEquals("Incorrect text when no past appointments. ",
                expectedNoPastText, actualNoPastText)
    }

    @Step
    fun checkUpcomingAppointmentsAreCorrectlyPopulated() {
        myAppointmentsPage.upcomingAppointmentsHeading.element.isDisplayed
        checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        )
    }

    @Step
    fun checkHistoricalAppointmentsAreCorrectlyPopulated() {
        myAppointmentsPage.pastAppointmentsHeading.element.isDisplayed
        checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS
        )
    }

    private fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
            sessionVariableKey: MyAppointmentsFactory.Expectations
    ) {
        val expectedSlots = Serenity.sessionVariableCalled<List<Slot>>(sessionVariableKey).map { slot ->
            slot.copy(id = null)
        }
        val areCliniciansExpected = expectedSlots.isNotEmpty() && expectedSlots[0].clinicians.isNotEmpty()
        val slots = when (sessionVariableKey) {
            MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS ->
                myAppointmentsPage.getAllUpcomingSlots(areCliniciansExpected)
            MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS ->
                myAppointmentsPage.getAllHistoricalSlots(areCliniciansExpected)
            else -> null
        }
        assertNotNull("Invalid session variable key. ", slots)
        assertEquals("Expected upcoming Appointments size doesn't match with the actual size",
                expectedSlots.size, slots!!.size)
        assertEquals("Exact expected Appointments list not found. ", HashSet(expectedSlots), HashSet(slots))
    }

    @Step
    fun checkIfUpcomingSlotsAreInCorrectOrder() {
        val slotDate = myAppointmentsPage.getDateTimestampsOfUpcomingSlots()
        Assert.assertTrue(
                "Upcoming slots are in the wrong order. ",
                Ordering.natural<Long>().isOrdered(slotDate)
        )
    }

    @Step
    fun checkIfHistoricalSlotsAreInCorrectOrder() {
        val slotDate = myAppointmentsPage.getDateTimestampsOfHistoricalSlots()
        Assert.assertTrue(
                "Historical slots are in the wrong order. ",
                Ordering.natural<Long>().reverse<Long>().isOrdered(slotDate)
        )
    }

    @Step
    fun verifyCancellationConfirmationMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(cancellationSuccessMessage, message)
    }

    @Step
    fun verifyThatThereIsACancelLinkForEachUpcomingAppointment(appointmentsWithoutCancelLink: Int = 0) {
        val expectedNumberOfSlots = Serenity.sessionVariableCalled<List<Slot>>(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).size
        assertEquals(
                "Missing at least one cancel link. ",
                (expectedNumberOfSlots - appointmentsWithoutCancelLink),
                myAppointmentsPage.getNumberOfCancelLinks()
        )
        assertEquals(
                "Found a reference to not being able to cancel. ",
                appointmentsWithoutCancelLink,
                myAppointmentsPage.getNumberOfAppointmentsThatCannotBeCancelled()
        )
    }

    @Step
    fun verifyThatThereAreNoCancelLinks() {
        val expectedNumberOfSlots = Serenity.sessionVariableCalled<List<Slot>>(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).size
        assertEquals(
                "Missing a reference to not being able to cancel. ",
                expectedNumberOfSlots,
                myAppointmentsPage.getNumberOfAppointmentsThatCannotBeCancelled()
        )
        assertEquals(
                "Found a cancel link. ",
                0,
                myAppointmentsPage.getNumberOfCancelLinks()
        )
    }

    @Step
    fun checkAppointmentDataErrorMessagesAreCorrect() {
        val expectedHeader = "There's been a problem getting your appointment history"
        val expectedBody = "Try again later. If the problem continues and you need this information now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear()
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }
}
