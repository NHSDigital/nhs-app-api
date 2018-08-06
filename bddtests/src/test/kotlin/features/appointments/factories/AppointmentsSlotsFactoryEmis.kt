package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import java.time.Instant
import java.time.ZoneOffset
import java.time.zone.ZoneRulesProvider.getRules
import java.time.ZoneId
import java.time.OffsetDateTime
import java.util.*


class AppointmentsSlotsFactoryEmis: AppointmentsSlotsFactory("EMIS") {

    override fun generateAppointmentSlotResponse(startDate: String?,
                                                  endDate: String?,
                                                  mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {

        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
        mockingClient.forEmis {mapping(appointmentSlotsMetaRequest(patient, startDate, endDate))}
    }


    override val zoneOffset= getOffset()

    fun getOffset():ZoneOffset{
        val odt = OffsetDateTime.now(ZoneId.of("Europe/London"))
        return odt.offset
    }
}