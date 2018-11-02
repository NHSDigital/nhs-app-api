package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.serenitybdd.core.Serenity.setSessionVariable
import java.util.*


class AppointmentsBookingFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(guidanceMessage: Boolean = true, reasonNecessityOption: NecessityOption = NecessityOption.MANDATORY) {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateDefaultAvailableAppointmentSlotExample(guidanceMessage= guidanceMessage, reasonNecessity = reasonNecessityOption)
    }

    fun generateMultipleAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateMultipleAvailableAppointmentSlotsForTheSameTime()
    }

    fun generateSuccessfulBookingResponse(bookingReason: String = "Reason") {
        generateBookingResponse(bookingReason = bookingReason) { bookRequest -> bookRequest.respondWithSuccess() }
    }

    fun generateSuccessfulBookingResponseEmptyReasong() {
        generateSuccessfulBookingResponse(bookingReason = "")
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
                    "VISION" to { AppointmentsBookingFactory("VISION") },
                    "TPP" to { AppointmentsBookingFactory("TPP") })
        }

        const val SymptomsToEnter = "SymptomsToEnter"
    }
}