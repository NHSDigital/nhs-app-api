package mocking.data.appointments

import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mocking.stubs.appointments.AppointmentsSlotsExampleBuilder
import mocking.stubs.appointments.IdValue
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Channel
import worker.models.appointments.SlotResponseObject
import java.time.DayOfWeek.MONDAY
import java.time.LocalDateTime

class AppointmentsSlotsExample {

    companion object {
        private const val slot1ID = 301
        private const val slot2ID = 302
        private const val slot3ID = 303
        private const val tomorrow = 1L
        private const val dayAfterTomorrow = 2L
        private const val daysInWeek = 7L
        private const val defaultStartValue = 0
        private const val appointmentTimeHour = 14
        private const val appointmentTimeMin = 10

        private const val endOfDayTimeHour = 23
        private const val endOfDayTimeMin = 55
        private const val duration = 10

        private const val cancellationCutOff: Long = 30

        private val currentTime = LocalDateTime.now()
        private var currentDateToAdd = currentTime
        val remainingDatesForThisWeek = setWeek()
        val datesForNextWeek = setWeek()

        private val tomorrowDate = LocalDateTime.now().plusDays(1)

        private const val clinicSessionType = "Clinic"
        private const val clinicSlot = "Clinic - Slot"
        private const val telephoneAppointmentType = "Telephone"
        private const val telephoneSlot = "Telephone - Slot"

        private val locationLeeds = IdValue(1, "Leeds")
        private val locationSheffield = IdValue(2, "Sheffield")
        private val staffDrWho = IdValue(101, "Dr. Who")
        private val staffDrScott = IdValue(102, "Dr. Scott")
        private val channelTelephone: Channel = Channel.Telephone
        private val channelUnknown: Channel = Channel.Unknown

        private val startDateTimeForPastAppointment = DateTimeWrapper(currentTime.minusMinutes(10))
        private val endDateTimeForPastAppointment = DateTimeWrapper(currentTime)

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

        fun getExampleWithPastAppointment(): AppointmentSlotsResponseFacade {
            return getGenericExample(
                    arrayListOf(pastAppointmentSession).plus(appointmentSessions)
                            as java.util.ArrayList<AppointmentSessionFacade>
            )
        }

        val appointmentSessionWithinCutoffTime = AppointmentSessionFacadeBuilder()
                .sessionId(slot1ID)
                .sessionType(clinicSessionType)
                .staffDetails(arrayListOf(staffDrWho))
                .location(locationLeeds)
                .slots {
                    addAppointment {
                        slotId(slot1ID)
                                .startDate(DateTimeWrapper(
                                        LocalDateTime.now()
                                                .plusMinutes(cancellationCutOff)).dateTimeAsBackendString)
                                .endDate(DateTimeWrapper(
                                        LocalDateTime.now()
                                                .plusMinutes(cancellationCutOff + duration))
                                        .dateTimeAsBackendString)
                                .setSlotInThePast()
                    }
                }.build()

        fun getExampleWithAppointmentWithinCutoffTime(): AppointmentSlotsResponseFacade {
            return getGenericExample(
                    arrayListOf(appointmentSessionWithinCutoffTime).plus(appointmentSessions)
                            as java.util.ArrayList<AppointmentSessionFacade>
            )
        }

        fun slotExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(slot1ID)
                            .sessionType(telephoneAppointmentType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(slot1ID)
                                            .startDate(startDateAppointment1.dateTimeAsBackendString)
                                            .endDate(endDateAppointment1.dateTimeAsBackendString)
                                            .channel(channelTelephone)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(slot2ID)
                                    .sessionType(telephoneAppointmentType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(slot2ID)
                                                    .startDate(startDateAppointment2.dateTimeAsBackendString)
                                                    .endDate(endDateAppointment2.dateTimeAsBackendString)
                                                    .channel(channelTelephone)
                                        }
                                    }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(slot3ID)
                                    .sessionType(clinicSessionType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(slot3ID)
                                                    .startDate(startDateAppointment2.dateTimeAsBackendString)
                                                    .endDate(endDateAppointment2.dateTimeAsBackendString)
                                                    .channel(channelUnknown)
                                        }
                                    }.build()))
                    .filterValues(AppointmentFilterFacade(
                            type = telephoneSlot,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1)
                            )
                    ))
                    .appointmentTypesList(arrayListOf(telephoneSlot))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = slot1ID.toString(),
                                    type = telephoneSlot,
                                    startTime = startDateAppointment1.dateTimeAsBackendString,
                                    endTime = endDateAppointment1.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value),
                                    channel = 1
                            ), SlotResponseObject(
                                    id = slot2ID.toString(),
                                    type = telephoneSlot,
                                    startTime = startDateAppointment2.dateTimeAsBackendString,
                                    endTime = endDateAppointment2.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrScott.value),
                                    channel = 1
                            )))
                    .build()
        }

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

        fun getGenericExample(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>):
                AppointmentSlotsResponseFacade {
            return getGenericExampleBuilder()
                    .appointmentSessions(appointmentSessionFacade)
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
                            .sessionId(slot1ID)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(slot1ID)
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
                                    id = slot1ID.toString(),
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
                            .sessionId(slot1ID)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(slot1ID)
                                            .startDate(startDateAppointment1.dateTimeAsBackendString)
                                            .endDate(endDateAppointment1.dateTimeAsBackendString)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(slot2ID)
                                    .sessionType(clinicSessionType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(slot2ID)
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
                                    id = slot1ID.toString(),
                                    type = clinicSlot,
                                    startTime = startDateAppointment1.dateTimeAsBackendString,
                                    endTime = endDateAppointment1.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            ), SlotResponseObject(
                                    id = slot2ID.toString(),
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
                            .sessionId(slot1ID)
                            .sessionType(clinicSessionType)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(slot1ID)
                                            .startDate(startDateAppointment1.dateTimeAsBackendString)
                                            .endDate(endDateAppointment1.dateTimeAsBackendString)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(slot2ID)
                                    .sessionType(clinicSessionType)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(slot2ID)
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
                                            id = slot1ID.toString(),
                                            type = clinicSlot,
                                            startTime = startDateAppointment1.dateTimeAsBackendString,
                                            endTime = endDateAppointment1.dateTimeAsBackendString,
                                            location = locationLeeds.value,
                                            clinicians = arrayOf(staffDrWho.value)
                                    ),
                                    SlotResponseObject(
                                            id = slot2ID.toString(),
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
            val dayAfterTomorrow = LocalDateTime.now().plusDays(dayAfterTomorrow)
            val startDate = DateTimeWrapper(dayAfterTomorrow, appointmentTimeHour, appointmentTimeMin)
            val endDate = DateTimeWrapper(dayAfterTomorrow, appointmentTimeHour, duration)
            return singleSlotExample(startDate, endDate)
        }

        fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
            val startDate = DateTimeWrapper(LocalDateTime.now(), endOfDayTimeHour, endOfDayTimeMin)
            val endDate = DateTimeWrapper(LocalDateTime.now().plusDays(tomorrow), defaultStartValue, defaultStartValue)
            return singleSlotExample(startDate, endDate)
        }

        fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
            val aWeekToday = LocalDateTime.now().plusDays(daysInWeek)
            val startDate = DateTimeWrapper(aWeekToday, appointmentTimeHour, appointmentTimeMin)
            val endDate = DateTimeWrapper(aWeekToday, appointmentTimeHour, duration)
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
