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

    private val filter =
            AppointmentFilterFacade(
                    type = clinicSlotType,
                    doctor = staffDrWho.value,
                    location = locationLeeds.value,
                    filteredSlots = generateMapOfAppointmentDatesAndTimes(
                            arrayListOf(
                                    startDateAppointment1.sessionName(clinicSessionType),
                                    startDateAppointment2.sessionName(clinicSessionType)
                            )
                    )
            )

    fun getExampleWithAppointmentWithinCutoffTime(): AppointmentSlotsResponseFacade {
        return getGenericExample(
                arrayListOf(appointmentSessionWithinCutoffTime).plus(appointmentSessions)
                        as java.util.ArrayList<AppointmentSessionFacade>
        )
    }

    fun getExampleWithPastAppointment(): AppointmentSlotsResponseFacade {
        return getGenericExample(
                arrayListOf(pastAppointmentSession).plus(appointmentSessions)
                        as java.util.ArrayList<AppointmentSessionFacade>
        )
    }

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
                .appointmentTypesList(arrayListOf(clinicSlotType))
                .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                .locationsList(arrayListOf(locationLeeds.value, locationSheffield.value))
                .expectedResponseSlots(expectedResponseSlots)
    }

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
        private const val clinicSlotType = "Slot"
        private const val telephoneSessionType = "Telephone"
        private const val telephoneSlotType = "Slot"

        private val locationLeeds = IdValue(1, "Leeds")
        private val locationSheffield = IdValue(2, "Sheffield")
        private val staffDrWho = IdValue(101, "Dr. Who")
        private val staffDrScott = IdValue(102, "Dr. Scott")
        private val channelTelephone: Channel = Channel.Telephone
        private val channelUnknown: Channel = Channel.Unknown

        private val startDateTimeForPastAppointment = FilterSlotDetails(currentTime.minusMinutes(10))
        private val endDateTimeForPastAppointment = FilterSlotDetails(currentTime)

        private val startDateAppointment1 = FilterSlotDetails(tomorrowDate, 14, 0)
        private val endDateAppointment1 = FilterSlotDetails(tomorrowDate, 14, 10)

        private val startDateAppointment2 = FilterSlotDetails(tomorrowDate, 15, 20)
        private val endDateAppointment2 = FilterSlotDetails(tomorrowDate, 15, 30)

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

        val appointmentSessionWithinCutoffTime = AppointmentSessionFacadeBuilder()
                .sessionId(slot1ID)
                .sessionType(clinicSessionType)
                .staffDetails(arrayListOf(staffDrWho))
                .location(locationLeeds)
                .slots {
                    addAppointment {
                        slotId(slot1ID)
                                .startDate(FilterSlotDetails(
                                        LocalDateTime.now()
                                                .plusMinutes(cancellationCutOff)).dateTimeAsBackendString)
                                .endDate(FilterSlotDetails(
                                        LocalDateTime.now()
                                                .plusMinutes(cancellationCutOff + duration))
                                        .dateTimeAsBackendString)
                                .setSlotInThePast()
                    }
                }.build()

        val historicalAppointmentSession = AppointmentSessionFacadeBuilder()
                .sessionId(slot2ID)
                .sessionType(clinicSessionType)
                .staffDetails(arrayListOf(staffDrWho))
                .location(locationLeeds)
                .slots {
                    addAppointment {
                        slotId(slot2ID)
                                .startDate(FilterSlotDetails(
                                        LocalDateTime.now().minusMonths(6)).dateTimeAsBackendString
                                )
                                .endDate(
                                        FilterSlotDetails(
                                                LocalDateTime.now()
                                                        .minusMonths(6)
                                                        .plusMinutes(duration.toLong())
                                        )
                                                .dateTimeAsBackendString
                                )
                                .setSlotInThePast()
                    }
                }.build()

        fun slotExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(slot1ID)
                            .sessionType(telephoneSessionType)
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
                                    .sessionType(telephoneSessionType)
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
                            type = telephoneSlotType,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1.sessionName(telephoneSessionType))
                            )
                    ))
                    .appointmentTypesList(arrayListOf(telephoneSlotType))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = slot1ID.toString(),
                                    type = telephoneSlotType,
                                    sessionName = telephoneSessionType,
                                    startTime = startDateAppointment1.dateTimeAsBackendString,
                                    endTime = endDateAppointment1.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value),
                                    channel = 1
                            ), SlotResponseObject(
                                    id = slot2ID.toString(),
                                    type = telephoneSlotType,
                                    sessionName = telephoneSessionType,
                                    startTime = startDateAppointment2.dateTimeAsBackendString,
                                    endTime = endDateAppointment2.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrScott.value),
                                    channel = 1
                            )))
                    .build()
        }



        private val expectedResponseSlots = arrayListOf(
                SlotResponseObject(
                        id = "301",
                        type = clinicSlotType,
                        sessionName = clinicSessionType,
                        startTime = startDateAppointment1.dateTimeAsBackendString,
                        endTime = endDateAppointment1.dateTimeAsBackendString,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "302",
                        type = clinicSlotType,
                        sessionName = clinicSessionType,
                        startTime = startDateAppointment2.dateTimeAsBackendString,
                        endTime = endDateAppointment2.dateTimeAsBackendString,
                        location = locationLeeds.value,
                        clinicians = arrayOf(staffDrWho.value)
                ),
                SlotResponseObject(
                        id = "401",
                        type = clinicSlotType,
                        sessionName = clinicSessionType,
                        startTime = startDateAppointment1.dateTimeAsBackendString,
                        endTime = endDateAppointment1.dateTimeAsBackendString,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                ),
                SlotResponseObject(
                        id = "402",
                        type = clinicSlotType,
                        sessionName = clinicSessionType,
                        startTime = startDateAppointment2.dateTimeAsBackendString,
                        endTime = endDateAppointment2.dateTimeAsBackendString,
                        location = locationSheffield.value,
                        clinicians = arrayOf(staffDrScott.value)
                )
        )

        fun singleSlotExample(startDate: FilterSlotDetails = startDateAppointment1,
                              endDate: FilterSlotDetails = endDateAppointment1)
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
                            type = clinicSlotType,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDate.sessionName(clinicSessionType))
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlotType))
                    .cliniciansList(arrayListOf(staffDrWho.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = slot1ID.toString(),
                                    type = clinicSlotType,
                                    sessionName = clinicSessionType,
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
                            type = clinicSlotType,
                            doctor = staffDrWho.value,
                            location = locationLeeds.value,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1.sessionName(clinicSessionType))
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlotType))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(SlotResponseObject(
                                    id = slot1ID.toString(),
                                    type = clinicSlotType,
                                    sessionName = clinicSessionType,
                                    startTime = startDateAppointment1.dateTimeAsBackendString,
                                    endTime = endDateAppointment1.dateTimeAsBackendString,
                                    location = locationLeeds.value,
                                    clinicians = arrayOf(staffDrWho.value)
                            ), SlotResponseObject(
                                    id = slot2ID.toString(),
                                    type = clinicSlotType,
                                    sessionName = clinicSessionType,
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
                            type = clinicSlotType,
                            filteredSlots = generateMapOfAppointmentDatesAndTimes(
                                    arrayListOf(startDateAppointment1.sessionName(clinicSessionType))
                            )
                    ))
                    .appointmentTypesList(arrayListOf(clinicSlotType))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .expectedResponseSlots(
                            arrayListOf(
                                    SlotResponseObject(
                                            id = slot1ID.toString(),
                                            type = clinicSlotType,
                                            sessionName = clinicSessionType,
                                            startTime = startDateAppointment1.dateTimeAsBackendString,
                                            endTime = endDateAppointment1.dateTimeAsBackendString,
                                            location = locationLeeds.value,
                                            clinicians = arrayOf(staffDrWho.value)
                                    ),
                                    SlotResponseObject(
                                            id = slot2ID.toString(),
                                            type = clinicSlotType,
                                            sessionName = clinicSessionType,
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
            val startDate = FilterSlotDetails(dayAfterTomorrow, appointmentTimeHour, appointmentTimeMin)
            val endDate = FilterSlotDetails(dayAfterTomorrow, appointmentTimeHour, duration)
            return singleSlotExample(startDate, endDate)
        }

        fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
            val startDate = FilterSlotDetails(LocalDateTime.now(), endOfDayTimeHour, endOfDayTimeMin)
            val endDate = FilterSlotDetails(
                    LocalDateTime.now().plusDays(tomorrow),
                    defaultStartValue,
                    defaultStartValue
            )
            return singleSlotExample(startDate, endDate)
        }

        fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
            val aWeekToday = LocalDateTime.now().plusDays(daysInWeek)
            val startDate = FilterSlotDetails(aWeekToday, appointmentTimeHour, appointmentTimeMin)
            val endDate = FilterSlotDetails(aWeekToday, appointmentTimeHour, duration)
            return singleSlotExample(startDate, endDate)
        }

        private fun generateMapOfAppointmentDatesAndTimes(arrayOfFilteredSlots: ArrayList<FilterSlotDetails>)
                : Map<String, Set<FilterSlotDetails>> {
            var result = mapOf<String, Set<FilterSlotDetails>>()
            for (slotDetails in arrayOfFilteredSlots) {
                val date = slotDetails.dateAsUIString
                val currentSetOfSlots = result[date] ?: setOf()
                result = result.plus(Pair(date, currentSetOfSlots.plus(slotDetails)))
            }
            return result
        }

        private fun setWeek(currentArray: ArrayList<String> = arrayListOf()): ArrayList<String> {
            val timeOfDay = FilterSlotDetails(currentDateToAdd)
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
