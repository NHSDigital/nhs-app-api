package features.im1Appointments.steps

import com.google.common.collect.Ordering
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import pages.appointments.YourAppointmentsPage
import pages.isDisplayed
import pages.navigation.HeaderNative
import pages.navigation.WebHeader
import java.util.*

open class YourAppointmentsUISteps {

    lateinit var yourAppointmentsPage: YourAppointmentsPage
    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader

    private val pageHeader = "Your GP appointments"

    private val backLink = "Go to your appointments"

    private val expectedNoUpcomingText = "Upcoming appointments\n" +
            "If you have an upcoming appointment that is not shown here, contact your GP surgery for more information."
    private val expectedNoPastText = "Past appointments\n" +
            "You have no recent past appointments. To find out about older appointments, contact your GP surgery."

    @Step
    fun checkBackToAppointmentsLink() {
        val actualMessage = yourAppointmentsPage.getBackLink()
        assertEquals(backLink, actualMessage)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        yourAppointmentsPage.locatorMethods.assertNativeElementsLoaded(yourAppointmentsPage.bookButton)
        yourAppointmentsPage.
                bookButton.click()
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        webHeader.getPageTitle().withText(pageHeader)
    }

    @Step
    fun checkNoUpcomingAppointmentsTextIsDisplaying() {
        val actualNoUpcomingText = yourAppointmentsPage.getNoUpcomingText()
        assertEquals("Incorrect text when no upcoming appointments. ",
                expectedNoUpcomingText, actualNoUpcomingText)
    }

    @Step
    fun checkNoHistoricalAppointmentsTextIsDisplaying() {
        val actualNoPastText = yourAppointmentsPage.getNoPastText()
        assertEquals("Incorrect text when no past appointments. ",
                expectedNoPastText, actualNoPastText)
    }

    @Step
    fun checkUpcomingAppointmentsAreCorrectlyPopulated() {
        yourAppointmentsPage.upcomingAppointmentsHeading.isDisplayed
        checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        )
    }

    @Step
    fun checkHistoricalAppointmentsAreCorrectlyPopulated() {
        yourAppointmentsPage.pastAppointmentsHeading.isDisplayed
        checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS
        )
    }

    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated(
            sessionVariableKey: MyAppointmentsFactory.Expectations, isTelephoneAppointment: Boolean = false
    ) {
        val expectedSlots = Serenity.sessionVariableCalled<List<Slot>>(sessionVariableKey).map { slot ->
            slot.copy(id = null)
        }
        val areCliniciansExpected = expectedSlots.isNotEmpty() && expectedSlots[0].clinicians.isNotEmpty()
        val slots = when (sessionVariableKey) {
            MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS ->
                yourAppointmentsPage.getAllUpcomingSlots(areCliniciansExpected, isTelephoneAppointment)
            MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS ->
                yourAppointmentsPage.getAllHistoricalSlots(areCliniciansExpected, isTelephoneAppointment)
            else -> null
        }
        assertNotNull("Invalid session variable key. ", slots)
        assertEquals("Expected Appointments size doesn't match with the actual size",
                expectedSlots.size, slots!!.size)
        assertEquals("Exact expected Appointments list not found. ", HashSet(expectedSlots), HashSet(slots))
    }

    @Step
    fun checkIfUpcomingSlotsAreInCorrectOrder() {
        val slotDate = yourAppointmentsPage.getDateTimestampsOfUpcomingSlots()
        Assert.assertTrue(
                "Upcoming slots are in the wrong order. ",
                Ordering.natural<Long>().isOrdered(slotDate)
        )
    }

    @Step
    fun checkIfHistoricalSlotsAreInCorrectOrder() {
        val slotDate = yourAppointmentsPage.getDateTimestampsOfHistoricalSlots()
        Assert.assertTrue(
                "Historical slots are in the wrong order. ",
                Ordering.natural<Long>().reverse<Long>().isOrdered(slotDate)
        )
    }

    @Step
    fun verifyThatThereIsACancelLinkForEachUpcomingAppointment(appointmentsWithoutCancelLink: Int = 0) {
        val expectedNumberOfSlots = Serenity.sessionVariableCalled<List<Slot>>(
                MyAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).size
        assertEquals(
                "Missing at least one cancel link. ",
                (expectedNumberOfSlots - appointmentsWithoutCancelLink),
                yourAppointmentsPage.getNumberOfCancelLinks()
        )
        assertEquals(
                "Found a reference to not being able to cancel. ",
                appointmentsWithoutCancelLink,
                yourAppointmentsPage.getNumberOfAppointmentsThatCannotBeCancelled()
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
                yourAppointmentsPage.getNumberOfAppointmentsThatCannotBeCancelled()
        )
        assertEquals(
                "Found a cancel link. ",
                0,
                yourAppointmentsPage.getNumberOfCancelLinks()
        )
    }
}
