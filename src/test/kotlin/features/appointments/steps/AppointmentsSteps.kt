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
    fun selectSlot() {
        appointments.selectFirstSlot()
    }

    @Step
    fun clickOnBookAppointmentButton() {
        appointments.clickOnBookAppointmentButton()
    }
}
