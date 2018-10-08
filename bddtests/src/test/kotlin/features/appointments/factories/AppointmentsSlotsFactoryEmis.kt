package features.appointments.factories

import features.appointments.steps.AvailableAppointmentsSteps
import mocking.emis.models.Messages
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import java.time.ZoneOffset
import java.time.ZoneId
import java.time.OffsetDateTime

import net.serenitybdd.core.Serenity
import org.junit.Assert.assertTrue
import java.util.TimeZone

class AppointmentsSlotsFactoryEmis : AppointmentsSlotsFactory("EMIS") {

    override fun generateAppointmentSlotResponse(startDate: String?, endDate: String?, guidanceMessage: Boolean, mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        generateAppointmentSlotResponseWithoutGuidance(startDate, endDate, mapping)

        val settingsResponse = if (guidanceMessage) {
            SettingsResponseModel()
        } else {
            val messages = Messages(appointmentsMessage = "")
            SettingsResponseModel(messages = messages)
        }

        val appointmentsMessage = settingsResponse.messages.appointmentsMessage

        assertTrue("Appointment message incorrectly being stubbed: $appointmentsMessage.", guidanceMessage == appointmentsMessage.isNotEmpty())

        mockingClient.forEmis {
            practiceSettingsRequest(patient)
                    .respondWithSuccess(settingsResponse)
        }

        Serenity.setSessionVariable(AvailableAppointmentsSteps.AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
                .to(appointmentsMessage)
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: String?,
                                                                endDate: String?,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {

        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
        mockingClient.forEmis { mapping(appointmentSlotsMetaRequest(patient, startDate, endDate)) }
    }

    override val supplierAdjustTime= TimeZone.getTimeZone("Europe/London").toZoneId()

    private fun getOffset():ZoneOffset{
        val odt = OffsetDateTime.now(ZoneId.of("Europe/London"))
        return odt.offset
    }
}