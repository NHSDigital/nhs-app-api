package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.JSonXmlConverter
import mocking.commonData.BaseAppointmentData
import mocking.emis.data.EmisAppointmentData
import mocking.models.Mapping
import models.Patient

class ViewAppointmentsFactoryEmis : ViewAppointmentsFactory() {

    override fun createUpcomingAppointments(patient: Patient?): String {
        return JSonXmlConverter.toJsonWithUpperCamelCase(
                EmisAppointmentData.instance.createGetAppointmentsResponse())
    }

    override fun createEmptyUpcomingAppointmentResponse(patient: Patient?): String {
        return JSonXmlConverter.toJsonWithUpperCamelCase(
                EmisAppointmentData.instance.createGetAppointmentsResponseForNoUpcomingAppointments())
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forEmis { resolver() }
    }

    override fun getAppointmentData(): BaseAppointmentData {
        return EmisAppointmentData.instance
    }
}