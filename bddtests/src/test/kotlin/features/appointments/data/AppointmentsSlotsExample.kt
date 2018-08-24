package features.appointments.data

import constants.AppointmentDateTimeFormat
import features.appointments.factories.AppointmentSessionFacadeBuilder
import features.appointments.factories.IdValue
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import worker.models.appointments.SlotResponseObject
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class AppointmentsSlotsExample {

    companion object {

        private val dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)
        private val tomorrowDate = LocalDateTime.now().plusDays(1)

        private const val clinic = "Clinic"
        private const val clinicSlot = "Clinic - Slot"

        private val locationLeeds = IdValue (1, "Leeds")
        private val locationSheffield = IdValue (2, "Sheffield")
        private val staffDrWho = IdValue (101, "Dr. Who")
        private val staffDrScott = IdValue (102, "Dr. Scott")

        private var startDateAppointment1 = tomorrowDate.withHour(14).withMinute(0).format(dateFormatter)
        private var endDateAppointment1 = tomorrowDate.withHour(14).withMinute(10).format(dateFormatter)

        private var startDateAppointment2 = tomorrowDate.withHour(15).withMinute(20).format(dateFormatter)
        private var endDateAppointment2 = tomorrowDate.withHour(15).withMinute(30).format(dateFormatter)

        private val appointmentSessions = arrayListOf(AppointmentSessionFacadeBuilder()
                .sessionId(301)
                .sessionType(clinic)
                .staffDetails(staffDrWho)
                .location(locationLeeds)
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
                }.build(),
                AppointmentSessionFacadeBuilder()
                .sessionId(402)
                .sessionType(clinic)
                .staffDetails(staffDrScott)
                .location(locationSheffield)
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
                }.build())

        private val filter =
                AppointmentFilterFacade(
                        type = clinicSlot,
                        doctor = staffDrWho.value,
                        location = locationLeeds.value
                )

        private val expectedResponseSlots = arrayListOf(
                SlotResponseObject(
                        id = "301",
                        type = clinicSlot,
                        startTime = startDateAppointment1,
                        endTime = endDateAppointment1,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "302",
                        type = clinicSlot,
                        startTime = startDateAppointment2,
                        endTime = endDateAppointment2,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "401",
                        type = clinicSlot,
                        startTime = startDateAppointment1,
                        endTime = endDateAppointment1,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                ),
                SlotResponseObject(
                        id = "402",
                        type = clinicSlot,
                        startTime = startDateAppointment2,
                        endTime = endDateAppointment2,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                )
        )

        fun getGenericExample(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilder()
                    .appointmentSessions(appointmentSessions)
                    .filterValues(filter)
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value, locationSheffield.value))
                    .expectedResponseSlots(expectedResponseSlots)
                    .build()
        }


        fun singleSlotExample(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilder()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(301)
                            .sessionType(clinic)
                            .staffDetails(staffDrWho)
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(301)
                                            .startDate(startDateAppointment1)
                                            .endDate(endDateAppointment1)
                                }
                            }.build()))
                    .filterValues(AppointmentFilterFacade(
                            type = clinicSlot,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = "301",
                                    type = clinicSlot,
                                    startTime = startDateAppointment1,
                                    endTime = endDateAppointment1,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            )))
                    .build()
        }

        fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilder()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(301)
                            .sessionType(clinic)
                            .staffDetails(staffDrWho)
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(301)
                                            .startDate(startDateAppointment1)
                                            .endDate(endDateAppointment1)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(302)
                                    .sessionType(clinic)
                                    .staffDetails(staffDrScott)
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(302)
                                                    .startDate(startDateAppointment2)
                                                    .endDate(endDateAppointment2)
                                        }
                                    }.build()))
                    .filterValues(AppointmentFilterFacade(
                            type = clinicSlot,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = "301",
                                    type = clinicSlot,
                                    startTime = startDateAppointment1,
                                    endTime = endDateAppointment1,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            ),SlotResponseObject(
                                    id = "302",
                                    type = clinicSlot,
                                    startTime = startDateAppointment2,
                                    endTime = endDateAppointment2,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrScott.value)
                            )))
                    .build()
        }
    }
}