package features.informaticaAppointments.stepDefinitions

import io.cucumber.java.en.Then
import pages.appointments.InformaticaAppointmentsPage

class InformaticaAppointmentsStepDefinitions {

    lateinit var informatica : InformaticaAppointmentsPage

    @Then("^I see an appropriate message informing me that my GP surgery uses Appointments Online$")
    fun iSeeAnAppropriateMessageInformingMeThatMyGPSurgeryUsesAppointmentsOnline(){
        informatica.isLoaded()
        informatica.assertInformaticaAppointmentsPageVisible()
    }
}
