package features.appointments.steps.data

import net.thucydides.core.annotations.Step
import pages.navigation.MyAppointmentsPage

open class MyAppointmentsSteps {

    lateinit var myAppointments: MyAppointmentsPage

    @Step
    fun clickOnBookNewAppointmentButton() {
        myAppointments.clickOnBookNewAppointmentButton()
    }
}
