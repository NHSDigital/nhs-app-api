package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.emis.models.InputRequirements
import mocking.emis.practices.NecessityOption
import mocking.emis.practices.SettingsResponseModel
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import utils.ProxySerenityHelpers

class AppointmentsBookingFactoryEmis : AppointmentsBookingFactory(Supplier.EMIS) {

    override fun requiresBookingReason(boolean: Boolean) {
        val inputRequirements = InputRequirements(appointmentBookingReason = NecessityOption.MANDATORY.text)
        val settingsResponse = SettingsResponseModel(inputRequirements = inputRequirements)
        mockingClient.forEmis.mock {
            practiceSettingsRequest(ProxySerenityHelpers.getPatientOrProxy())
                    .respondWithSuccess(settingsResponse)
        }
    }

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason)
    }

    override fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int?,
                                             bookingReason: String?,
                                             telephoneNumber: String?,
                                             telephoneContactType: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                    patient.userPatientLinkToken,
                    slotId ?: defaultApptBookingSlotId,
                    bookingReason ?: defaultApptBookingReason,
                    telephoneNumber = telephoneNumber,
                    telephoneContactType = telephoneContactType)
    }
}