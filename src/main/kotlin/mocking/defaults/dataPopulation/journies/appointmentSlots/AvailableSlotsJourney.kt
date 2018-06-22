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
        val locations = arrayListOf(appointmentData.location1, appointmentData.location2)
        val sessionHolders = arrayListOf(
                appointmentData.sessionHolder1,
                appointmentData.sessionHolder2,
                appointmentData.sessionHolder3)

        val sessions = arrayListOf(
                appointmentData.session1,
                appointmentData.session2,
                appointmentData.session3)

        val appointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                locations = locations,
                sessionHolders = sessionHolders,
                sessions = sessions)

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