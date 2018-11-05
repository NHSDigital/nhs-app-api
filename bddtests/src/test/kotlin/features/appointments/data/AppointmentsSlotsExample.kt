package features.appointments.data

import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mocking.stubs.appointments.AppointmentsSlotsExampleBuilder
import mocking.stubs.appointments.IdValue
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import worker.models.appointments.SlotResponseObject
import java.time.DayOfWeek.MONDAY
import java.time.LocalDateTime

class AppointmentsSlotsExample {

    companion object {

        private var currentDateToAdd = LocalDateTime.now()
        val remainingDatesForThisWeek = setWeek()
        val datesForNextWeek = setWeek()

        private val tomorrowDate = LocalDateTime.now().plusDays(1)

        private const val clinicSessionType = "Clinic"
        private const val clinicSlot = "Clinic - Slot"

        private val locationLeeds = IdValue(1, "Leeds")
        private val locationSheffield = IdValue(2, "Sheffield")
        private val staffDrWho = IdValue(101, "Dr. Who")
        private val staffDrScott = IdValue(102, "Dr. Scott")

        private val startDateTimeForPastAppointment = DateTimeWrapper(LocalDateTime.now().minusMinutes(10))
        private val endDateTimeForPastAppointment = DateTimeWrapper(LocalDateTime.now())

        private val startDateAppointment1 = DateTimeWrapper(tomorrowDate, 14, 0)
        private val endDateAppointment1 = DateTimeWrapper(tomorrowDate, 14, 10)

        private val startDateAppointment2 = DateTimeWrapper(tomorrowDate, 15, 20)
        private val endDateAppointment2 = DateTimeWrapper(tomorrowDate, 15, 30)

        private val appointmentSessions = arrayListOf(
                AppointmentSessionFacadeBuilder()
                        .sessionId(102)
                        .sessionType(clinicSessionType)
                        .staffDetails(arrayListOf(staffDrWho))
                        .location(locationLeeds)
                        .slots {
                            addAppointment {
                                slotId(301)
                                        .startDate(startDateAppointment1.dateTimeAsBackendString)
                                        .endDate(endDateAppointment1.dateTimeAsBackendString)
                            }
                                    .addAppointment {
                                        slotId(302)
                                                .startDate(startDateAppointment2.dateTimeAsBackendString)
                                                .endDate(endDateAppointment2.dateTimeAsBackendString)
                                    }
                        }.build(),
                AppointmentSessionFacadeBuilder()
                        .sessionId(103)
                        .sessionType(clinicSessionType)
                        .staffDetails(arrayListOf(staffDrScott))
                        .location(locationSheffield)
                        .slots {
                            addAppointment {
                                slotId(401)
                                        .startDate(startDateAppointment1.dateTimeAsBackendString)
                                        .endDate(endDateAppointment1.dateTimeAsBackendString)
                            }
                                    .addAppointment {
                                        slotId(402)
                                                .startDate(startDateAppointment2.dateTimeAsBackendString)
                                                .endDate(endDateAppointment2.dateTimeAsBackendString)
                                    }
                        }.build()
        )

        private val pastAppointmentSession = AppointmentSessionFacadeBuilder()
                .sessionId(101)
                .sessionType(clinicSessionType)
                .staffDetails(arrayListOf(staffDrWho))
                .location(locationLeeds)
                .slots {
                    addAppointment {
                        slotId(201)
                                .startDate(startDateTimeForPastAppointment.dateTimeAsBackendString)
                                .endDate(endDateTimeForPastAppointment.dateTimeAsBackendString)
                                .setSlotInThePast()
                    }
                }.build()

        private val filter =
                AppointmentFilterFacade(
                        type = clinicSlot,
                        doctor = staffDrWho.value,
                        location = locationLeeds.value,
                        filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                arrayListOf(startDateAppointment1, startDateAppointment2)
                        )
                )

        private val expectedResponseSlots = arrayListOf(
                SlotResponseObject(
                        id = "301",
                        type = clinicSlot,
                        startTime = startDateAppointment1.dateTimeAsBackendString,
                        endTime = endDateAppointment1.dateTimeAsBackendString,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "302",
                        type = clinicSlot,
                        startTime = startDateAppointment2.dateTimeAsBackendString,
                        endTime = endDateAppointment2.dateTimeAsBackendString,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "401",
                        type = clinicSlot,
                        startTime = startDateAppointment1.dateTimeAsBackendString,
                        endTime = endDateAppointment1.dateTimeAsBackendString,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                ),
                SlotResponseObject(
                        id = "402",
                        type = clinicSlot,
                        startTime = startDateAppointment2.dateTimeAsBackendString,
                        endTime = endDateAppointment2.dateTimeAsBackendString,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                )
        )

        fun getGenericExample(): AppointmentSlotsResponseFacade {
            return getGenericExampleBuilder()
                    .build()
        }

        fun getFacadeWithPastAppointment(): AppointmentSlotsResponseFacade {
            return getGenericExampleBuilder()
                    .appointmentSessions(
                            arrayListOf(pastAppointmentSession).plus(appointmentSessions)
                                    as java.util.ArrayList<AppointmentSessionFacade>
                    )
                    .build()
        }

        private fun getGenericExampleBuilder(): AppointmentsSlotsExampleBuilder {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(appointmentSessions)
                    .filterValues(filter)
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value, locationSheffield.value))
                    .expectedResponseSlots(expectedResponseSlots)
        }

        fun singleSlotExample(startDate: DateTimeWrapper = startDateAppointment1,
                              endDate: DateTimeWrapper = endDateAppointment1)
                : AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(301)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(301)
                                            .startDate(startDate.dateTimeAsBackendString)
                                            .endDate(endDate.dateTimeAsBackendString)
                                }
                            }
                            .build()))
                    .filterValues(AppointmentFilterFacade(
                            type = clinicSlot,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDate)
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = "301",
                                    type = clinicSlot,
                                    startTime = startDate.dateTimeAsBackendString,
                                    endTime = endDate.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            )))
                    .build()
        }

        fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(301)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(301)
                                            .startDate(startDateAppointment1.dateTimeAsBackendString)
                                            .endDate(endDateAppointment1.dateTimeAsBackendString)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(302)
                                    .sessionType(clinicSessionType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(302)
                                                    .startDate(startDateAppointment2.dateTimeAsBackendString)
                                                    .endDate(endDateAppointment2.dateTimeAsBackendString)
                                        }
                                    }.build()))
                    .filterValues(AppointmentFilterFacade(
                            type = clinicSlot,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1)
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = "301",
                                    type = clinicSlot,
                                    startTime = startDateAppointment1.dateTimeAsBackendString,
                                    endTime = endDateAppointment1.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            ), SlotResponseObject(
                                    id = "302",
                                    type = clinicSlot,
                                    startTime = startDateAppointment2.dateTimeAsBackendString,
                                    endTime = endDateAppointment2.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrScott.value)
                            )))
                    .build()
        }

        fun multipleSlotsOneTime(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(301)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(301)
                                            .startDate(startDateAppointment1.dateTimeAsBackendString)
                                            .endDate(endDateAppointment1.dateTimeAsBackendString)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(302)
                                    .sessionType(clinicSessionType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(302)
                                                    .startDate(startDateAppointment1.dateTimeAsBackendString)
                                                    .endDate(endDateAppointment1.dateTimeAsBackendString)
                                        }
                                    }.build()))
                    .filterValues(AppointmentFilterFacade(
                            type = clinicSlot,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1)
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(
                                    SlotResponseObject(
                                            id = "301",
                                            type = clinicSlot,
                                            startTime = startDateAppointment1.dateTimeAsBackendString,
                                            endTime = endDateAppointment1.dateTimeAsBackendString,
                                            location = locationLeeds.value,
                                            clinicians = arrayOf(staffDrWho.value)
                                    ),
                                    SlotResponseObject(
                                            id = "302",
                                            type = clinicSlot,
                                            startTime = startDateAppointment1.dateTimeAsBackendString,
                                            endTime = endDateAppointment1.dateTimeAsBackendString,
                                            location = locationLeeds.value,
                                            clinicians = arrayOf(staffDrScott.value)
                                    )
                            ))
                    .build()
        }

        fun slotForDayAfterTomorrow(): AppointmentSlotsResponseFacade {
            val dayAfterTomorrow = LocalDateTime.now().plusDays(2)
            val startDate = DateTimeWrapper(dayAfterTomorrow, 14, 0)
            val endDate = DateTimeWrapper(dayAfterTomorrow, 14, 10)
            return singleSlotExample(startDate, endDate)
        }

        fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
            val startDate = DateTimeWrapper(LocalDateTime.now(), 23, 55)
            val endDate = DateTimeWrapper(LocalDateTime.now().plusDays(1), 0, 0)
            return singleSlotExample(startDate, endDate)
        }

        fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
            val aWeekToday = LocalDateTime.now().plusDays(7)
            val startDate = DateTimeWrapper(aWeekToday, 14, 0)
            val endDate = DateTimeWrapper(aWeekToday, 14, 10)
            return singleSlotExample(startDate, endDate)
        }

        private fun generateMapOfAppointmentDatesAndTimes(arrayOfDateTimes: ArrayList<DateTimeWrapper>)
                : Map<String, Set<String>> {
            var result = mapOf<String, Set<String>>()
            for (dateTime in arrayOfDateTimes) {
                val date = dateTime.dateAsUIString
                val time = dateTime.timeAsUIString
                val currentSetOfTimes = result[date] ?: setOf()
                result = result.plus(Pair(date, currentSetOfTimes.plus(time)))
            }
            return result
        }

        private fun setWeek(currentArray: ArrayList<String> = arrayListOf()): ArrayList<String> {
            val timeOfDay = DateTimeWrapper(currentDateToAdd)
            currentArray.add(timeOfDay.dateAsUIString)
            currentDateToAdd = currentDateToAdd.plusDays(1)
            return if (currentDateToAdd.dayOfWeek != MONDAY) {
                setWeek(currentArray)
            } else {
                currentArray
            }
        }
    }
}

