package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
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
import utils.ProxySerenityHelpers
import worker.models.appointments.AppointmentBookRequest

abstract class AppointmentsBookingFactory(gpSupplier: Supplier) : AppointmentsFactory(gpSupplier) {

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
        val patient = ProxySerenityHelpers.getPatientOrProxy()
        val request = defaultAppointmentRequest(patient)
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    fun telephoneAppointmentBookingSetupWithResult(
            telephoneNumber: String?,
            emptyBookingReason: Boolean = false,
            slotId: Int? = null,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        val patient = ProxySerenityHelpers.getPatientOrProxy()
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
        val appointmentRequest = AppointmentBookRequest(
                toBeBooked.userPatientLinkToken,
                toBeBooked.slotId.toString(),
                toBeBooked.bookingReason,
                toBeBooked.startTime,
                toBeBooked.endTime,
                toBeBooked.telephoneNumber,
                toBeBooked.telephoneContactType
        )
        Serenity.setSessionVariable(appointmentToBookKey).to(appointmentRequest)
        Serenity.setSessionVariable(symptomsToEnter).to(toBeBooked.bookingReason)
    }

    private fun generateBookingResponse(bookingReason: String, booker: (IBookAppointmentsBuilder) ->
    Mapping) {
        val patient = ProxySerenityHelpers.getPatientOrProxy()
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

    open fun requiresBookingReason(boolean: Boolean = true){}

    companion object : SupplierSpecificFactory<AppointmentsBookingFactory>() {

        override val map: HashMap<Supplier, (() -> AppointmentsBookingFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { AppointmentsBookingFactoryEmis() },
                            Supplier.TPP to { AppointmentsBookingFactoryTpp() },
                            Supplier.VISION to { AppointmentsBookingFactoryVision() },
                            Supplier.MICROTEST to { AppointmentsBookingFactoryMicrotest()})
                }


        const val appointmentToBookKey = "appointmentToBook"
        const val symptomsToEnter = "symptomsToEnter"
        const val telephoneNumberToEnter = "telephoneNumberToEnter"
        const val selectedSlot = "selectedSlot"

        const val defaultApptBookingReason = "I have a bad back."
        const val defaultApptBookingSlotId = 12345
        const val defaultTelephoneNumber = "12345678"
        const val defaultTelephoneContactType = "Other"
    }
}
