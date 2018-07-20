package features.appointments.stepDefinitions

import mocking.MockingClient
import mocking.IBookAppointmentsBuilder
import mocking.IAppointmentMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentBookRequest
import org.junit.Assert.*

abstract class AppointmentsBookingFactory {

    val mockingClient = MockingClient.instance

    fun defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        var patient = getDefaultPatient()
        var request = defaultAppointmentRequest(patient)
        sendRequestViaMockingClient { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    abstract fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int? = null,
                                           bookingReason: String? = null): BookAppointmentSlotFacade

    abstract fun getDefaultPatient(): Patient

    fun setupRequestAndResponse(request: BookAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
            sendRequestViaMockingClient(response)
        }
        setAppointmentToBeBooked(request)
    }

    protected abstract fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping)

    private fun setAppointmentToBeBooked(toBeBooked: BookAppointmentSlotFacade) {
        Serenity.setSessionVariable("AppointmentToBook").to(getAppointmentBookRequest(toBeBooked))
    }

    private fun getAppointmentBookRequest(bookApptSlot: BookAppointmentSlotFacade): AppointmentBookRequest {
        return AppointmentBookRequest(
                bookApptSlot.slotId.toString(),
                bookApptSlot.bookingReason,
                bookApptSlot.startTime,
                bookApptSlot.endTime
        )
    }

    companion object {

        private val map: HashMap<String, AppointmentsBookingFactory> =
                hashMapOf(
                        "EMIS" to AppointmentsBookingFactoryEmis(),
                        "TPP" to AppointmentsBookingFactoryTpp())

        fun getForSupplier(gpSystem: String): AppointmentsBookingFactory {
            if(! map.containsKey(gpSystem))
            {
                fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem)
        }

        val defaultApptBookingReason = "I have a bad back."
        val defaultApptBookingSlotId = 12345
    }
}