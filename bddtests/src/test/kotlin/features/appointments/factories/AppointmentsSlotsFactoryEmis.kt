package features.appointments.factories

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.emis.models.InputRequirements
import mocking.emis.models.Messages
import mocking.emis.practices.NecessityOption
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertTrue
import java.time.ZonedDateTime

class AppointmentsSlotsFactoryEmis : AppointmentsSlotsFactory("EMIS") {

    override fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: String?,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        generateAppointmentSlotResponseWithoutGuidance(startDate, endDate, mapping)

        val guidanceMessageOut = guidanceMessage ?: Messages().appointmentsMessage
        val inputRequirements = InputRequirements(appointmentBookingReason = reasonNecessity.text)
        val messages = Messages(appointmentsMessage = guidanceMessageOut)
        val settingsResponse = SettingsResponseModel(messages = messages, inputRequirements = inputRequirements)

        val appointmentsMessage = settingsResponse.messages.appointmentsMessage

        assertTrue("Appointment message incorrectly being stubbed: $appointmentsMessage.",
                guidanceMessageOut!!.isNotEmpty() == appointmentsMessage!!.isNotEmpty())

        mockingClient.forEmis {
            practiceSettingsRequest(patient)
                    .respondWithSuccess(settingsResponse)
        }

        Serenity.setSessionVariable(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
                .to(appointmentsMessage)
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
        mockingClient.forEmis {
            mapping(appointments.appointmentSlotsMetaRequest(patient, startDate, endDate))
        }
    }

    override fun getExpectedApiResponseSlots(facade: AppointmentSlotsResponseFacade) =
            appointmentSlotsFactoryHelper.getExpectedApiResponseSlotsWithSessionNames(
                    facade, true
            )

    override fun getExpectedUiRepresentationOfFilteredSlots(facade: AppointmentFilterFacade) =
            appointmentSlotsFactoryHelper.getExpectedUiRepresentationOfFilteredSlotsWithSessionNames(
                    facade, true
            )
}
