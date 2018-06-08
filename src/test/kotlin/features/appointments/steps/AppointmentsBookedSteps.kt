package features.appointments.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.AppointmentsBookedPage

open class AppointmentsBookedSteps {

    lateinit var appointmentsBookedPage: AppointmentsBookedPage

    @Step
    fun checkSuccessMessage() {
        val message = appointmentsBookedPage.getSuccessMessage()
        Assert.assertTrue(message.contains("Appointment Booked"))
    }
}
