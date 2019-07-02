package features.im1Appointments.factories

import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.ZonedDateTime

class AppointmentsSlotsFactoryMicrotest : AppointmentsSlotsFactory("MICROTEST") {

    override fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: String?,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping) {
        generateAppointmentSlotResponseWithoutGuidance(startDate, endDate, mapping)
    }

    override fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        appointmentMapper.requestMapping {
            mapping(appointmentSlotsRequest(patient, startDate, endDate))
        }
        mockingClient.forMicrotest {
            mapping(appointments.appointmentSlotsRequest(patient, startDate, endDate))
        }
    }

    override fun getExpectedApiResponseSlots(facade: AppointmentSlotsResponseFacade) =
            appointmentSlotsFactoryHelper.getExpectedApiResponseSlotsWithSessionNames(
                    facade, false
            )

    override fun getExpectedUiRepresentationOfFilteredSlots(facade: AppointmentFilterFacade) =
            appointmentSlotsFactoryHelper.getExpectedUiRepresentationOfFilteredSlotsWithSessionNames(
                    facade, false
            )
}
