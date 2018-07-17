package features.appointments.data

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.JSonXmlConverter
import mocking.commonData.BaseAppointmentData
import mocking.models.Mapping
import mocking.tpp.data.TppAppointmentData
import models.Patient

class ViewAppointmentsFactoryTpp : ViewAppointmentsFactory() {

    override fun createUpcomingAppointments(patient: Patient?): String {
        val selectedPatient = patient ?: getAppointmentData().defaultPatient
        return JSonXmlConverter.toXML(TppAppointmentData.instance.createTppAppointmentsResponse(selectedPatient))
    }

    override fun createEmptyUpcomingAppointmentResponse(patient: Patient?): String {
        val selectedPatient = patient ?: getAppointmentData().defaultPatient
        return JSonXmlConverter.toXML(TppAppointmentData.instance.createEmptyTppMyAppointmentResponse(selectedPatient))
    }

    override fun getAppointmentData(): BaseAppointmentData {
        return TppAppointmentData.instance
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forTpp { resolver() }
    }
}