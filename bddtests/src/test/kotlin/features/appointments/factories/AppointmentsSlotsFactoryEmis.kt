package features.appointments.factories

import features.appointments.steps.AvailableAppointmentsSteps
import mocking.emis.practices.NecessityOption
import mocking.emis.models.InputRequirements
import mocking.emis.models.Messages
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertTrue
import java.util.TimeZone

class AppointmentsSlotsFactoryEmis : AppointmentsSlotsFactory("EMIS") {

    override fun generateAppointmentSlotResponse(startDate: String?, endDate: String?, guidanceMessage: Boolean, reasonNecessity: NecessityOption, mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        generateAppointmentSlotResponseWithoutGuidance(startDate, endDate, mapping)

        val inputRequirements = InputRequirements(appointmentBookingReason = reasonNecessity.text)
        val settingsResponse = if (guidanceMessage) {
            SettingsResponseModel(inputRequirements = inputRequirements)
        } else {
            val messages = Messages(appointmentsMessage = "")
            SettingsResponseModel(messages = messages, inputRequirements = inputRequirements)
        }

        val appointmentsMessage = settingsResponse.messages.appointmentsMessage

        assertTrue("Appointment message incorrectly being stubbed: $appointmentsMessage.", guidanceMessage == appointmentsMessage.isNotEmpty())

        mockingClient.forEmis {
            appointments.practiceSettingsRequest(patient)
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
        mockingClient.forEmis { mapping(appointments.appointmentSlotsMetaRequest(patient, startDate, endDate)) }
    }

    override val supplierAdjustTime = TimeZone.getTimeZone("Europe/London").toZoneId()
}
