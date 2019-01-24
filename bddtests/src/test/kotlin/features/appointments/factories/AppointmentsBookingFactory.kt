package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import java.util.*

class AppointmentsBookingFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(guidanceMessage: String? = null,
                                                       reasonNecessityOption: NecessityOption =
                                                               NecessityOption.MANDATORY) {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateDefaultAvailableAppointmentSlotExample(guidanceMessage = guidanceMessage,
                reasonNecessity = reasonNecessityOption)
    }

    fun generateMultipleAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateMultipleAvailableAppointmentSlotsForTheSameTime()
    }

    fun generateAvailableSlotExampleIncludingTelephoneAppointment(guidanceMessage: String? = null,
                                                       reasonNecessityOption: NecessityOption =
                                                               NecessityOption.MANDATORY,
                                                         telephoneNumberToEnter: String="") {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateAvailableSlotExampleIncludingTelephoneAppointment(guidanceMessage = guidanceMessage,
                reasonNecessity = reasonNecessityOption)

        setSessionVariable(TelephoneNumberToEnter).to(telephoneNumberToEnter)
    }

    fun generateSuccessfulBookingResponse(bookingReason: String = "Reason") {
        generateBookingResponse(bookingReason = bookingReason) {
            bookRequest -> bookRequest
                .respondWithSuccess()
                .inScenario("Appointments")
                .willSetStateTo("Appointment Booked")
        }
    }

    fun generateSuccessfulBookingResponseEmptyReason() {
        generateSuccessfulBookingResponse(bookingReason = "")
    }

    fun generateBookingResponse(booker: (IBookAppointmentsBuilder) -> Mapping) {
        generateBookingResponse(bookingReason = "Reason", booker = booker)
    }

    private fun generateBookingResponse(bookingReason: String, booker: (IBookAppointmentsBuilder) ->
    Mapping) {
        val sessionToSelect = Serenity.sessionVariableCalled<List<AppointmentSessionFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .APPOINTMENT_SLOTS_EXAMPLE_SESSIONS
        ).first()
        val slotToSelect = sessionToSelect.slots.first()
        setSessionVariable(SelectedSlot).to(getExpectedUiRepresentationOfSlot(slotToSelect, sessionToSelect))
        appointmentMapper.requestMapping {
            booker(bookAppointmentSlotRequest(patient,
                    BookAppointmentSlotFacade(patient.userPatientLinkToken, slotToSelect.slotId!!.toInt(),
                    bookingReason))
            )
        }
        setSessionVariable(SymptomsToEnter).to(bookingReason)
    }

    companion object : SupplierSpecificFactory<AppointmentsBookingFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsBookingFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsBookingFactory("EMIS") },
                    "TPP" to { AppointmentsBookingFactory("TPP") },
                    "VISION" to { AppointmentsBookingFactory("VISION") })
        }

        const val SymptomsToEnter = "SymptomsToEnter"
        const val TelephoneNumberToEnter = "TelephoneNumberToEnter"
        const val SelectedSlot = "SelectedSlot"
    }
}
