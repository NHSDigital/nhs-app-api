package features.appointments.steps

import models.Slot
import net.thucydides.core.annotations.Step
import org.hamcrest.Matcher
import org.junit.Assert
import pages.AppointmentsPage

open class AppointmentsSteps {

    lateinit var appointments: AppointmentsPage

    @Step
    fun slots(matches: Matcher<ArrayList<Slot>>) {
        Assert.assertThat(appointments.getAllSlots(), matches)
    }

    @Step
    fun checkIfSlotsAreDisplayed() {
        Assert.assertTrue(appointments.countSlots() > 0)
    }

    @Step
    fun selectSlot() {
        appointments.selectFirstSlot()
    }

    @Step
    fun clickOnBookAppointmentButton() {
        appointments.clickOnBookAppointmentButton()
    }

    @Step
    fun checkTimeoutErrorMessage() {
        val message = appointments.getServerErrorElement()
        Assert.assertTrue(message.text.contains("Sorry, there's been a problem loading this page"))
    }

    @Step
    fun checkIfTyAgainButtonDisplayed() {
        val button = appointments.getTryAgainButton()
        Assert.assertEquals("Try again", button.text)
        Assert.assertTrue(button.isDisplayed)
    }

    @Step
    fun checkIfTyAgainButtonIsNotDisplayed() {
        val hasButton = appointments.hasTryAgainButton()
        Assert.assertFalse(hasButton)
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val message = appointments.getServerErrorElement()
        Assert.assertTrue(message.text.contains("Sorry, there's been a problem loading this page"))
    }

    @Step
    fun clickOnTryAgainButton() {
        val button = appointments.getTryAgainButton()
        button.click()
    }
}
