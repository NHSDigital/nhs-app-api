package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping

class AppointmentsSlotsFactoryEmis: AppointmentsSlotsFactory("EMIS") {

    override fun generateAppointmentSlotResponse(startDate: String?,
                                                  endDate: String?,
                                                  mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {

        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
        mockingClient.forEmis {mapping(appointmentSlotsMetaRequest(patient, startDate, endDate))}
    }
}