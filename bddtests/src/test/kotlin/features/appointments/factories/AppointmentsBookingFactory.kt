package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.serenitybdd.core.Serenity.setSessionVariable
import java.util.*


class AppointmentsBookingFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample() {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateDefaultAvailableAppointmentSlotExample()
    }

    fun generateMultipleAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateMultipleAvailableAppointmentSlotsForTheSameTime()
    }

    fun generateSuccessfulBookingResponse(bookingReason: String) {
        generateBookingResponse(bookingReason = bookingReason) { bookRequest -> bookRequest.respondWithSuccess() }
    }

    fun generateSuccessfulBookingResponse() {
        generateSuccessfulBookingResponse(bookingReason = "Reason")
    }

    fun generateBookingResponse(booker: (IBookAppointmentsBuilder) -> Mapping) {
        generateBookingResponse(bookingReason = "Reason", booker = booker)
    }

    fun generateBookingResponse(slotId: Int = 301, bookingReason: String, booker: (IBookAppointmentsBuilder) -> Mapping) {
        appointmentMapper.requestMapping {
            booker(bookAppointmentSlotRequest(patient,
                    BookAppointmentSlotFacade(patient.userPatientLinkToken, slotId, bookingReason))
            )
        }
        setSessionVariable(SymptomsToEnter).to(bookingReason)
    }

    companion object: SupplierSpecificFactory<AppointmentsBookingFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsBookingFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsBookingFactory("EMIS") },
                    "TPP" to { AppointmentsBookingFactory("TPP") })
        }

        const val SymptomsToEnter = "SymptomsToEnter"
    }
}