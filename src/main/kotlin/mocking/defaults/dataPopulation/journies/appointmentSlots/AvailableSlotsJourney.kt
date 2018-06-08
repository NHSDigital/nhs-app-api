package mocking.defaults.dataPopulation.journies.appointmentSlots

import addDays
import addHours
import addMinutes
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.appointments.PostAppointmentRequestModel
import mocking.emis.appointments.PostAppointmentResponseModel
import mocking.emis.models.*
import reset
import java.text.SimpleDateFormat
import java.util.*

const val LOCATION_ID_SURGERY: Int = 1
const val LOCATION_ID_HOSPITAL: Int = 2

const val CLINICIAN_ID_DRSMITH: Int = 1
const val CLINICIAN_ID_NURSEJONES = 2
const val CLINICIAN_ID_MSBROWN = 3

const val SESSION_ID_FOOTCLINIC = 1
const val SESSION_ID_EYECLINIC = 2
const val SESSION_ID_EARCLINIC = 3

class AvailableSlotsJourney(private val client: MockingClient) {

    fun create() {
        val now = Date()
        val calendar = Calendar.getInstance(TimeZone.getTimeZone("utc"))

        val dateTimeFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")

        val locations = arrayListOf(
                Location(locationId = LOCATION_ID_SURGERY, locationName = "Main Surgery"),
                Location(locationId = LOCATION_ID_HOSPITAL, locationName = "Hospital")
        )

        val sessionHolders = arrayListOf(
                SessionHolder(clinicianId = CLINICIAN_ID_DRSMITH, displayName = "Dr. Smith"),
                SessionHolder(clinicianId = CLINICIAN_ID_NURSEJONES, displayName = "Nurse Jones"),
                SessionHolder(clinicianId = CLINICIAN_ID_MSBROWN, displayName = "Ms. Brown")
        )

        val sessions = arrayListOf(
                Session(
                        sessionId = SESSION_ID_FOOTCLINIC,
                        sessionName = "Nose clinic",
                        sessionType = SessionType.Timed,
                        locationId = LOCATION_ID_SURGERY,
                        clinicianIds = arrayListOf(CLINICIAN_ID_DRSMITH)),
                Session(
                        sessionId = SESSION_ID_EYECLINIC,
                        sessionName = "Eye clinic",
                        sessionType = SessionType.Timed,
                        locationId = LOCATION_ID_HOSPITAL,
                        clinicianIds = arrayListOf(CLINICIAN_ID_NURSEJONES, CLINICIAN_ID_MSBROWN)),
                Session(
                        sessionId = SESSION_ID_EARCLINIC,
                        sessionName = "Ear clinic",
                        sessionType = SessionType.Timed,
                        locationId = LOCATION_ID_SURGERY,
                        clinicianIds = arrayListOf(CLINICIAN_ID_MSBROWN))
        )

        val appointmentSessions = arrayListOf(
                AppointmentSession(
                        sessionId = SESSION_ID_FOOTCLINIC,
                        sessionDate = dateTimeFormat.format(calendar.addDays(1).time),
                        slots = arrayListOf(
                                AppointmentSlot(
                                        slotId = 1,
                                        startTime = dateTimeFormat.format(calendar.time),
                                        endTime = dateTimeFormat.format(calendar.addMinutes(15).time)
                                ), AppointmentSlot(
                                slotId = 2,
                                startTime = dateTimeFormat
                                        .format(calendar.reset(now).addDays(1).addHours(1).time),
                                endTime = dateTimeFormat
                                        .format(calendar.addMinutes(1).time)
                        )
                        )
                ),
                AppointmentSession(
                        sessionId = SESSION_ID_EYECLINIC,
                        sessionDate = dateTimeFormat.format(calendar.reset(now).addDays(2).time),
                        slots = arrayListOf(
                                AppointmentSlot(
                                        slotId = 3,
                                        startTime = dateTimeFormat.format(calendar.time),
                                        endTime = dateTimeFormat.format(calendar.addMinutes(15).time)
                                ), AppointmentSlot(
                                slotId = 4,
                                startTime = dateTimeFormat.format(calendar.addHours(1).time),
                                endTime = dateTimeFormat
                                        .format(calendar.addMinutes(20).time)
                        )
                        )
                ),
                AppointmentSession(
                        sessionId = SESSION_ID_EARCLINIC,
                        sessionDate = dateTimeFormat.format(calendar.reset(now).addDays(4).time),
                        slots = arrayListOf(
                                AppointmentSlot(
                                        slotId = 5,
                                        startTime = dateTimeFormat.format(calendar.time),
                                        endTime = dateTimeFormat.format(calendar.addMinutes(15).time)
                                )
                        )
                )
        )

        val patient = SuccessfulRegistrationJourney.patient

        val bookAppointmentRequests = arrayListOf<PostAppointmentRequestModel>()
        for (session in appointmentSessions) {
            for (slot in session.slots) {
                val postAppointmentRequestModel = PostAppointmentRequestModel(
                        patient.userPatientLinkToken,
                        slot.slotId!!,
                        ".+"
                )
                bookAppointmentRequests.add(postAppointmentRequestModel)
            }
        }

        client
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(
                                    GetAppointmentSlotsMetaResponseModel(
                                            locations = locations,
                                            sessionHolders = sessionHolders,
                                            sessions = sessions
                                    )
                            )
                }

        client
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWithSuccess(GetAppointmentSlotsResponseModel(appointmentSessions))
                }

        for (bookingRequest in bookAppointmentRequests) {
            client
                    .forEmis {
                        appointmentRequest(bookingRequest)
                                .respondWithSuccess(PostAppointmentResponseModel())
                    }
        }

    }
}