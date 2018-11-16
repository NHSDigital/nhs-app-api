package features.appointments.factories

import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import java.time.ZonedDateTime

class AppointmentsSlotsFactoryVision : AppointmentsSlotsFactory("VISION") {

    override fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: Boolean,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        throw NotImplementedError("Test Setup Incorrect: Practice Settings are not relevant to Vision anyway. ")
    }
}
