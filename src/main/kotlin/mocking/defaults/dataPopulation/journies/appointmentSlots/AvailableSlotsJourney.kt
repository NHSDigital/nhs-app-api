package mocking.defaults.dataPopulation.journies.appointmentSlots

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.appointments.PostAppointmentRequestModel
import mocking.emis.appointments.PostAppointmentResponseModel
import mocking.emis.data.AppointmentData
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import models.Patient

class AvailableSlotsJourney(private val client: MockingClient) {
    private val appointmentData = AppointmentData.instance

    fun create() {
        val appointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                locations = appointmentData.locations,
                sessionHolders = appointmentData.sessionHolders,
                sessions = appointmentData.sessions)

        val patient = SuccessfulRegistrationJourney.patient

        client.forEmis {
            appointmentGetRequest(patient)
                    .respondWithSuccess(AppointmentData.instance.createGetAppointmentsResponse())
        }

        client.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWithSuccess(appointmentSlotsMetaResponseModel)
        }

        val appointmentSessions = appointmentData.createAppointmentSessions()
        val appointmentSlotsResponseModel = GetAppointmentSlotsResponseModel(appointmentSessions)
        client.forEmis {
            appointmentSlotsRequest(patient)
                    .respondWithSuccess(appointmentSlotsResponseModel)
        }

        val bookAppointmentRequests = createBookAppointmentRequest(appointmentSessions, patient)
        for (bookingRequest in bookAppointmentRequests) {
            client.forEmis {
                appointmentRequest(bookingRequest)
                        .respondWithSuccess(PostAppointmentResponseModel())
            }
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