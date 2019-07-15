package features.im1Appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import worker.models.appointments.AppointmentBookRequest

abstract class AppointmentsBookingFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

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
                                                                          NecessityOption.MANDATORY) {
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateAvailableSlotExampleIncludingTelephoneAppointment(guidanceMessage = guidanceMessage,
                reasonNecessity = reasonNecessityOption, telephoneNumber = telephoneNumberToEnter)

        setSessionVariable(telephoneNumberToEnter).to(telephoneNumberToEnter)
    }

    fun generateSuccessfulBookingResponse(bookingReason: String = defaultApptBookingReason) {
        generateBookingResponse(bookingReason = bookingReason) { bookRequest ->
            bookRequest
                    .respondWithSuccess()
                    .inScenario("Appointments")
                    .willSetStateTo("Appointment Booked")
        }
    }

    fun generateBookingResponse(booker: (IBookAppointmentsBuilder) -> Mapping) {
        generateBookingResponse(bookingReason = defaultApptBookingReason, booker = booker)
    }

    fun defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {

        val request = defaultAppointmentRequest(patient)
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    fun telephoneAppointmentBookingSetupWithResult(
            telephoneNumber: String?,
            emptyBookingReason: Boolean = false,
            slotId: Int? = null,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {

        val request = telephoneAppointmentRequest(
                patient,
                slotId ?: getSlotToSelect().slotId,
                if (emptyBookingReason) "" else defaultApptBookingReason,
                telephoneNumber = telephoneNumber,
                telephoneContactType = if (telephoneNumber != null) defaultTelephoneContactType else null
        )
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    fun setupRequestAndResponse(request: BookAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
            appointmentMapper.requestMapping { response() }
        }
        setAppointmentToBeBooked(request)
    }

    private fun setAppointmentToBeBooked(toBeBooked: BookAppointmentSlotFacade) {
        Serenity.setSessionVariable(appointmentToBookKey).to(getAppointmentBookRequest(toBeBooked))
        Serenity.setSessionVariable(symptomsToEnter).to(toBeBooked.bookingReason)
    }

    private fun getAppointmentBookRequest(bookApptSlot: BookAppointmentSlotFacade): AppointmentBookRequest {
        return AppointmentBookRequest(
                bookApptSlot.userPatientLinkToken,
                bookApptSlot.slotId.toString(),
                bookApptSlot.bookingReason,
                bookApptSlot.startTime,
                bookApptSlot.endTime,
                bookApptSlot.telephoneNumber,
                bookApptSlot.telephoneContactType
        )
    }

    private fun generateBookingResponse(bookingReason: String, booker: (IBookAppointmentsBuilder) ->
    Mapping) {

        appointmentMapper.requestMapping {
            booker(bookAppointmentSlotRequest(patient,
                    BookAppointmentSlotFacade(patient.userPatientLinkToken, getSlotToSelect().slotId!!.toInt(),
                            bookingReason))
            )
        }
        setSessionVariable(symptomsToEnter).to(bookingReason)
    }

    private fun getSlotToSelect(): AppointmentSlotFacade {
        val sessionToSelect = Serenity.sessionVariableCalled<List<AppointmentSessionFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .APPOINTMENT_SLOTS_EXAMPLE_SESSIONS
        ).first()
        val slotToSelect = sessionToSelect.slots.first()
        setSessionVariable(selectedSlot).to(getExpectedUiRepresentationOfSlot(slotToSelect, sessionToSelect))
        return slotToSelect
    }

    abstract fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int? = null,
                                           bookingReason: String? = null): BookAppointmentSlotFacade

    abstract fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int? = null,
                                             bookingReason: String? = null,
                                             telephoneNumber: String? = null,
                                             telephoneContactType: String? = null): BookAppointmentSlotFacade

    companion object : SupplierSpecificFactory<AppointmentsBookingFactory>() {

        override val map: HashMap<String, (() -> AppointmentsBookingFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { AppointmentsBookingFactoryEmis() },
                            "TPP" to { AppointmentsBookingFactoryTpp() },
                            "VISION" to { AppointmentsBookingFactoryVision() },
                            "MICROTEST" to { AppointmentsBookingFactoryMicrotest()})
                }


        const val appointmentToBookKey = "appointmentToBook"
        const val symptomsToEnter = "symptomsToEnter"
        const val telephoneNumberToEnter = "telephoneNumberToEnter"
        const val telephoneNumberValueToEnter = "01642 849 894"
        const val selectedSlot = "selectedSlot"

        const val defaultApptBookingReason = "I have a bad back."
        const val defaultApptBookingSlotId = 12345
        const val defaultTelephoneNumber = "12345678"
        const val defaultTelephoneContactType = "Other"
    }
}
