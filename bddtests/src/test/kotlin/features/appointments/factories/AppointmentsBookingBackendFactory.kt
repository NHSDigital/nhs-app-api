package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentBookRequest

abstract class AppointmentsBookingBackendFactory(gpSupplier:String): AppointmentsFactory(gpSupplier) {

    fun defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {

        val request = defaultAppointmentRequest(patient)
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    fun telephoneAppointmentBookingSetupWithResult(
                        bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping, telephoneNumber: String) {

        val request = telephoneAppointmentRequest(patient, telephoneNumber = telephoneNumber)
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
    }

    abstract fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int? = null,
                                           bookingReason: String? = null): BookAppointmentSlotFacade

    abstract fun telephoneAppointmentRequest(patient: Patient,
                                           slotId: Int? = null,
                                           bookingReason: String? = null,
                                           telephoneNumber: String? = null,
                                           telephoneContactType: String? = null): BookAppointmentSlotFacade



    fun setupRequestAndResponse(request: BookAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
            appointmentMapper.requestMapping { response()}
        }
        setAppointmentToBeBooked(request)
    }


    private fun setAppointmentToBeBooked(toBeBooked: BookAppointmentSlotFacade) {
            Serenity.setSessionVariable(appointmentToBookKey).to(getAppointmentBookRequest(toBeBooked))
    }

    private fun getAppointmentBookRequest(bookApptSlot: BookAppointmentSlotFacade): AppointmentBookRequest {
        return AppointmentBookRequest(
                bookApptSlot.userPatientLinkToken,
                bookApptSlot.slotId.toString(),
                bookApptSlot.bookingReason,
                bookApptSlot.startTime,
                bookApptSlot.endTime
        )
    }

    companion object : SupplierSpecificFactory<AppointmentsBookingBackendFactory>(){

        override val map: HashMap<String, (()-> AppointmentsBookingBackendFactory)>
               by lazy{ hashMapOf(
                        "EMIS" to { AppointmentsBookingBackendFactoryEmis() },
                        "TPP" to { AppointmentsBookingBackendFactoryTpp() },
                        "VISION" to { AppointmentsBookingBackendFactoryVision() })}


        const val appointmentToBookKey = "AppointmentToBook"
        const val defaultApptBookingReason = "I have a bad back."
        const val defaultApptBookingSlotId = 12345
        const val defaultTelephoneNumber = "12345678"
        const val defaultTelephoneContactType = "Other"
    }
}