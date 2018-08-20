package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import models.Patient
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentBookRequest

abstract class AppointmentsBookingBackendFactory(gpSupplier:String): AppointmentsFactory(gpSupplier) {

    fun defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {

        val request = defaultAppointmentRequest(patient)
        appointmentMapper.requestMapping { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(request)
    }

    abstract fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int? = null,
                                           bookingReason: String? = null): BookAppointmentSlotFacade



    fun setupRequestAndResponse(request: BookAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
            appointmentMapper.requestMapping { response()}
        }
        setAppointmentToBeBooked(request)
    }


    private fun setAppointmentToBeBooked(toBeBooked: BookAppointmentSlotFacade) {
        Serenity.setSessionVariable("AppointmentToBook").to(getAppointmentBookRequest(toBeBooked))
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
                        "TPP" to { AppointmentsBookingBackendFactoryTpp() })}


        val defaultApptBookingReason = "I have a bad back."
        val defaultApptBookingSlotId = 12345
    }
}