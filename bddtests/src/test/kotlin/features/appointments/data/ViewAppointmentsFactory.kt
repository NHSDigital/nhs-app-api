package features.appointments.data

import mocking.IAppointmentMappingBuilder
import mocking.MockingClient
import mocking.SERENITY_VARIABLE_GP_SERVICE_KEY
import mocking.commonData.BaseAppointmentData
import mocking.gpServiceBuilderInterfaces.IMyAppointmentsBuilder
import mocking.models.Mapping
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert.fail

abstract class ViewAppointmentsFactory {
    val mockingClient = MockingClient.instance

    fun setUpViewAppointmentsWithResult(gPService: String, myAppointmentBuilder: (IMyAppointmentsBuilder) -> Mapping) {
        var patient = getAppointmentData().defaultPatient
        sendRequestViaMockingClient { myAppointmentBuilder(viewAppointment(patient)) }

        Serenity.setSessionVariable(SERENITY_VARIABLE_GP_SERVICE_KEY).to(gPService)
    }

    fun setupViewAppointmentResponse(response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {
        if (response != null) {
            sendRequestViaMockingClient(response)
        }
    }

    abstract fun getAppointmentData(): BaseAppointmentData

    abstract fun createUpcomingAppointments(patient: Patient? = null): String

    abstract fun createEmptyUpcomingAppointmentResponse(patient: Patient? = null): String

    protected abstract fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping)

    companion object {
        private val map: HashMap<String, ViewAppointmentsFactory> = hashMapOf(
                "EMIS" to ViewAppointmentsFactoryEmis(),
                "TPP" to ViewAppointmentsFactoryTpp())

        fun getForSupplier(gpSystem: String): ViewAppointmentsFactory {
            if (!map.containsKey(gpSystem)) {
                fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem)
        }
    }

}