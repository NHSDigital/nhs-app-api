package features.appointments.factories

import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import java.util.TimeZone

class AppointmentsSlotsFactoryTpp : AppointmentsSlotsFactory("TPP") {

    override fun generateAppointmentSlotResponse(startDate: String?, endDate: String?, guidanceMessage: Boolean, reasonNecessity: NecessityOption, mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: String?,
                                                                endDate: String?,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        throw Exception("Test Setup Incorrect: Practice Settings are not relevant to TPP anyway. ")
    }
}
