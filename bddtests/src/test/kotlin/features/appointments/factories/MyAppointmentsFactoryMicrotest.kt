package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryMicrotest : MyAppointmentsFactory("MICROTEST") {
    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        // Not yet implemented for Microtest
        return MyAppointmentsResponse()
    }

    override fun getExpectedUiRepresentationOfUpcomingSlots(facade: MyAppointmentsFacade): List<Slot> {
        // Not yet implemented for Microtest
        return emptyList()
    }

    override fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot> {
        // Not yet implemented for Microtest
        return emptyList()
    }

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        // Not yet implemented for Microtest
        return emptyList()
    }
}
