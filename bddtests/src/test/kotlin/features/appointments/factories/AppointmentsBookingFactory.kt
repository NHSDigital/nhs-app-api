package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.serenitybdd.core.Serenity
import org.junit.Assert
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*


class AppointmentsBookingFactory(gpSupplier:String): AppointmentsFactory(gpSupplier) {

    private var tomorrowDate = LocalDateTime.now().plusDays(1)

    fun generateDefaultAvailableAppointmentSlotExample() {
        generateDefaultUserData()
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateDefaultAvailableAppointmentSlotExample()

        //Format like : Wednesday 1 August 2018
        val formatter = DateTimeFormatter.ofPattern("EEEE d MMMM yyyy")
        val day = tomorrowDate.format(formatter)
        Serenity.setSessionVariable(TargetAppointmentDateKey).to(day)
        Serenity.setSessionVariable(TargetAppointmentTimeKey).to("2:00 pm")
    }

    fun generateSuccessfulBookingResponse(bookingReason: String) {
        generateBookingResponse(bookingReason){ bookRequest->bookRequest.respondWithSuccess()}
    }

    fun generateSuccessfulBookingResponse() {
        generateSuccessfulBookingResponse("Reason")
    }

    fun generateBookingResponse(booker: (IBookAppointmentsBuilder) -> Mapping) {
        generateBookingResponse("Reason", booker)
    }

    fun generateBookingResponse(bookingReason: String, booker: (IBookAppointmentsBuilder) -> Mapping) {
        appointmentMapper.requestMapping {
            booker(bookAppointmentSlotRequest(patient,
                    BookAppointmentSlotFacade(patient.userPatientLinkToken, 301, bookingReason))
            )
        }
        Serenity.setSessionVariable(SymptomsToEnter).to(bookingReason)
    }
    companion object {

        private val map : HashMap<String, (() -> (AppointmentsBookingFactory))> by lazy {
                    hashMapOf(
                            "EMIS" to { AppointmentsBookingFactory("EMIS") },
                            "TPP" to { AppointmentsBookingFactory("TPP") })
        }

        fun getForSupplier(gpSystem: String): AppointmentsBookingFactory {
            if (!map.containsKey(gpSystem)) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem).invoke()
        }

        const val SymptomsToEnter = "SymptomsToEnter"
        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}