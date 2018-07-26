package mocking.defaults.dataPopulation.journies.appointmentSlots

import mocking.JSonXmlConverter
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.appointments.*
import mocking.emis.data.EmisAppointmentData
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class AvailableSlotsJourney(private val client: MockingClient) {
    private val appointmentData = EmisAppointmentData.instance
    private val locations = appointmentData.locations
    private val sessionHolders = appointmentData.sessionHolders
    private val sessions = appointmentData.sessions
    private val getAppointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
            locations = locations,
            sessionHolders = sessionHolders,
            sessions = sessions)
    private val appointmentSessions = appointmentData.createAppointmentSessions()

    fun create(
            patient: Patient = SuccessfulRegistrationJourney.patient,
            appointmentSlotsMetaResponse: GetAppointmentSlotsMetaResponseModel = getAppointmentSlotsMetaResponseModel,
            appointmentSlotsResponse: AppointmentSlotsResponseFacade = AppointmentSlotsResponseFacade(appointmentSessions),
            appointmentBookingResponse: GetAppointmentsResponseModel = appointmentData.createGetAppointmentsResponse()) {

        client.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWithSuccess(appointmentSlotsMetaResponse)
        }

        client.forEmis {
            appointmentSlotsRequest(patient)
                    .respondWithSuccess(appointmentSlotsResponse)
        }

        val bookAppointmentRequests = createBookAppointmentRequest(appointmentSessions, patient)
        for (bookingRequest in bookAppointmentRequests) {
            client.forEmis {
                viewMyAppointmentsRequest(patient)
                        .respondWithSuccess(JSonXmlConverter.toJsonWithUpperCamelCase(appointmentBookingResponse))
            }
        }

        //accept all requests
        client.forEmis {
            bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken, 123, "Reason"))
                    .respondWithSuccess()
        }

        // first slot can be cancelled with reason "No longer required"
        client.forEmis {
            cancelAppointmentRequest(patient, CancelAppointmentSlotFacade(
                    "No longer required",
                    1,
                    patient.userPatientLinkToken))
                        .respondWithSuccess()
        }
    }

    private fun createBookAppointmentRequest(appointmentSessions: ArrayList<AppointmentSessionFacade>, patient: Patient):
            ArrayList<PostAppointmentRequestModel> {
        val bookAppointmentRequests = arrayListOf<PostAppointmentRequestModel>()
        val appointmentSlots = getAppointmentSlotsFrom(appointmentSessions)
        appointmentSlots.forEach { slot ->
            val postAppointmentRequestModel = PostAppointmentRequestModel(
                    patient.userPatientLinkToken,
                    slot.slotId!!,
                    ".+"
            )
            bookAppointmentRequests.add(postAppointmentRequestModel)
        }
        return bookAppointmentRequests
    }

    private fun getAppointmentSlotsFrom(appointmentSessions: ArrayList<AppointmentSessionFacade>): List<AppointmentSlotFacade> {
        val slots = mutableListOf<AppointmentSlotFacade>()
        appointmentSessions.forEach { slots.addAll(it.slots) }
        return slots
    }
}