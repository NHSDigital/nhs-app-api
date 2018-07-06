package mocking.defaults.dataPopulation.journies.appointmentSlots

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.appointments.*
import mocking.emis.data.AppointmentData
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import models.Patient
import worker.models.appointments.BookAppointmentSlotRequest

class AvailableSlotsJourney(private val client: MockingClient) {
    private val appointmentData = AppointmentData.instance
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
            appointmentSlotsResponse: GetAppointmentSlotsResponseModel = GetAppointmentSlotsResponseModel(appointmentSessions),
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
                appointmentGetRequest(patient)
                        .respondWithSuccess(appointmentBookingResponse)
            }
        }

        //accept all requests
        client
                .forEmis { bookAppointmentSlotRequest(patient, BookAppointmentSlotRequest(patient.userPatientLinkToken, 123, "Reason"))
                        .respondWithSuccess()
                }

        // first slot can be cancelled with reason "No longer required"
        client.forEmis {
            cancelAppointmentRequest(patient, CancelAppointmentRequest(
                    CancellationReason = "No longer required",
                    SlotId = 1,
                    UserPatientLinkToken = patient.userPatientLinkToken))
                        .respondWithSuccess(DeleteAppointmentResponseModel(true))
        }
    }

    private fun createBookAppointmentRequest(appointmentSessions: ArrayList<AppointmentSession>, patient: Patient):
            ArrayList<PostAppointmentRequestModel> {
        val bookAppointmentRequests = arrayListOf<PostAppointmentRequestModel>()
        val appointmentSlots = getAppointmentSlotsFrom(appointmentSessions)
        appointmentSlots.forEach { slot ->
            val postAppointmentRequestModel = PostAppointmentRequestModel(
                    patient.userPatientLinkToken,
                    slot.slotId,
                    ".+"
            )
            bookAppointmentRequests.add(postAppointmentRequestModel)
        }
        return bookAppointmentRequests
    }

    private fun getAppointmentSlotsFrom(appointmentSessions: ArrayList<AppointmentSession>): List<AppointmentSlot> {
        val slots = mutableListOf<AppointmentSlot>()
        appointmentSessions.forEach { slots.addAll(it.slots) }
        return slots
    }
}