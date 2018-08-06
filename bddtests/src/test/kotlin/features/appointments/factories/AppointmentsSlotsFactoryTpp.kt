package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping

class AppointmentsSlotsFactoryTpp: AppointmentsSlotsFactory("TPP"){

     override fun generateAppointmentSlotResponse(startDate: String?,
                                                           endDate: String?,
                                                           mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {


        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
    }
}