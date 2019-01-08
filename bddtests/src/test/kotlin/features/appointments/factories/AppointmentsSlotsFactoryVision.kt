package features.appointments.factories

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.emis.models.Messages
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import net.serenitybdd.core.Serenity
import java.time.ZonedDateTime

class AppointmentsSlotsFactoryVision : AppointmentsSlotsFactory("VISION") {

    override fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: String?,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        Serenity.setSessionVariable(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
                .to(guidanceMessage ?: Messages().appointmentsMessage)
        generateAppointmentSlotResponseWithoutGuidance(startDate, endDate, mapping)
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
    }
}
