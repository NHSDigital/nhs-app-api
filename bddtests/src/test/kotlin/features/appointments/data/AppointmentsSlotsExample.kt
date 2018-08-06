package features.appointments.data

import constants.AppointmentDateTimeFormat
import features.appointments.factories.AppointmentSessionFacadeBuilder
import features.appointments.steps.AvailableAppointmentsSteps
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import worker.models.appointments.SlotResponseObject
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*

class AppointmentsSlotsExample {

    companion object {


        private var dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)

        private var tomorrowDate = LocalDateTime.now().plusDays(1)

        private var startDateAppointment1 = tomorrowDate.withHour(14).withMinute(0).format(dateFormatter)
        private var endDateAppointment1 = tomorrowDate.withHour(14).withMinute(10).format(dateFormatter)

        private var startDateAppointment2 = tomorrowDate.withHour(15).withMinute(20).format(dateFormatter)
        private var endDateAppointment2 = tomorrowDate.withHour(15).withMinute(30).format(dateFormatter)

        private val session1 = AppointmentSessionFacadeBuilder()
                .sessionId(301)
                .sessionType("Clinic")
                .staffDetails("Dr. Who", 101)
                .location("Leeds", 1)
                .slots {
                    addAppointment {
                        slotId(301)
                                .startDate(startDateAppointment1)
                                .endDate(endDateAppointment1)
                    }
                            .addAppointment {
                                slotId(302)
                                        .startDate(startDateAppointment2)
                                        .endDate(endDateAppointment2)
                            }
                }.build()

        private val session2 = AppointmentSessionFacadeBuilder()
                .sessionId(402)
                .sessionType("Clinic")
                .staffDetails("Dr. Scott", 102)
                .location("Sheffield", 2)
                .slots {
                    addAppointment {
                        slotId(401)
                                .startDate(startDateAppointment1)
                                .endDate(endDateAppointment1)
                    }
                            .addAppointment {
                                slotId(402)
                                        .startDate(startDateAppointment2)
                                        .endDate(endDateAppointment2)
                            }
                }.build()

        private val defaultAppointmentSessions = arrayListOf(session1, session2)

        private val defaultFilter =
                AppointmentFilterFacade(
                        type = "Clinic - Slot",
                        doctor = "Dr. Who",
                        location = "Leeds"
                )

        private val expectedResponseSlots = arrayListOf(
                SlotResponseObject(
                        id = "301",
                        type = "Clinic - Slot",
                        startTime = startDateAppointment1,
                        endTime = endDateAppointment1,
                        location = "Leeds",
                        clinicians = arrayOf("Dr. Who")
                ),
                SlotResponseObject(
                        id = "302",
                        type = "Clinic - Slot",
                        startTime = startDateAppointment2,
                        endTime = endDateAppointment2,
                        location = "Leeds",
                        clinicians = arrayOf("Dr. Who")
                ),
                SlotResponseObject(
                        id = "401",
                        type = "Clinic - Slot",
                        startTime = startDateAppointment1,
                        endTime = endDateAppointment1,
                        location = "Sheffield",
                        clinicians = arrayOf("Dr. Scott")
                ),
                SlotResponseObject(
                        id = "402",
                        type = "Clinic - Slot",
                        startTime = startDateAppointment2,
                        endTime = endDateAppointment2,
                        location = "Sheffield",
                        clinicians = arrayOf("Dr. Scott")
                )
        )

        private var appointmentTypesList: ArrayList<String> = arrayListOf("Clinic - Slot")
        private var locationsList: ArrayList<String> = arrayListOf("Leeds", "Sheffield")
        private var cliniciansList: ArrayList<String> = arrayListOf("Dr. Who", "Dr. Scott")


        fun getExample(): AppointmentSlotsResponseFacade {

            val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(defaultAppointmentSessions, "1")
            setExpectations(defaultAppointmentSessions)
            return getAppointmentSlotsResponseModel
        }


        fun setExpectations(appointmentSessions: ArrayList<AppointmentSessionFacade>) {
            val emisAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
            emisAppointmentSlots.addAll(appointmentSessions.flatMap { session -> session.slots })
            Serenity.setSessionVariable(AvailableAppointmentsSteps.EXPECTED_APPOINTMENT_SESSIONS_KEY).to(appointmentSessions)

            Serenity.setSessionVariable(EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).to(defaultFilter)
            Serenity.setSessionVariable(EXPECTED_APPOINTMENT_TYPE_KEY).to(appointmentTypesList)
            Serenity.setSessionVariable(EXPECTED_APPOINTMENT_LOCATIONS_KEY).to(locationsList)
            Serenity.setSessionVariable(EXPECTED_APPOINTMENT_CLINICIANS_KEY).to(cliniciansList)
            Serenity.setSessionVariable(EXPECTED_RESPONSE_SLOTS_KEY).to(expectedResponseSlots)
        }

        const val EXPECTED_APPOINTMENT_TYPE_KEY = "ExpectedAppointmentTypesKey"
        const val EXPECTED_APPOINTMENT_LOCATIONS_KEY = "ExpectedAppointmentLocationsKey"
        const val EXPECTED_APPOINTMENT_CLINICIANS_KEY = "ExpectedAppointmentCliniciansKey"
        const val EXPECTED_APPOINTMENT_FILTER_FACADE_KEY = "ExpectedAppointmentFilterFacadeKey"
        const val EXPECTED_RESPONSE_SLOTS_KEY = "ExpectedResponseSlotsKey"

    }
}