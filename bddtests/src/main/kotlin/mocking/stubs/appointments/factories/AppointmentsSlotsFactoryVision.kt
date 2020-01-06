package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.emis.models.Messages
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import utils.ProxySerenityHelpers
import java.time.ZonedDateTime

class AppointmentsSlotsFactoryVision : AppointmentsSlotsFactory(Supplier.VISION) {

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
            mapping(appointmentSlotsRequest(ProxySerenityHelpers.getPatientOrProxy(), startDate, endDate))
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
