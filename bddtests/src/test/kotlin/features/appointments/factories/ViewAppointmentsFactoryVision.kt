package features.appointments.factories

import mocking.JSonXmlConverter
import mocking.commonData.BaseAppointmentData
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping
import mocking.tpp.data.TppAppointmentData
import mocking.vision.data.VisionAppointmentData
import models.Patient

class ViewAppointmentsFactoryVision : ViewAppointmentsFactory() {

    override fun createUpcomingAppointments(patient: Patient?): String {
        val selectedPatient = patient ?: getAppointmentData().defaultPatient
        return ""
    }

    override fun createEmptyUpcomingAppointmentResponse(patient: Patient?): String {
        val selectedPatient = patient ?: getAppointmentData().defaultPatient
        return ""
    }

    override fun getAppointmentData(): BaseAppointmentData {
        return VisionAppointmentData.instance
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        //VERY TEMP
        mockingClient.forTpp { resolver() }
    }
}