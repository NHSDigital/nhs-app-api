package mocking.stubs.appointments

import constants.DateTimeFormats
import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import worker.models.appointments.SlotResponseObject
import java.time.Duration
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class AppointmentSlotsStubs (private val mockingClient: MockingClient) {

    fun generateEMISStubs() {
        val facade = multipleSlotsOneLocation()
        val mapAppointmentSlotsStubs =
                InputResponse<Patient, IAppointmentSlotsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(facade)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.withDelay(Duration.ofSeconds(TIMEOUT_DELAY))
                                    .respondWithSuccess(facade)
                        }

        mapAppointmentSlotsStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(appointmentSlotsRequest(scenario.forMatcher)) }
            mockingClient.forEmis {
                scenario.getResponse(appointmentSlotsMetaRequest(scenario.forMatcher))
            }
        }
    }

    companion object {

        private val dateFormatter = DateTimeFormatter.ofPattern(
                DateTimeFormats.backendDateTimeFormatWithoutTimezone
        )
        private val tomorrowDate = LocalDateTime.now().plusDays(1)

        private const val clinic = "Clinic"
        private const val clinicSlot = "Clinic - Slot"
        private const val firstSessionId = 301
        private const val secondSessionId = 302
        private const val firstSlotId = 301
        private const val secondSlotId = 302


        private val locationLeeds = IdValue(1, "Leeds")
        private val staffDrWho = IdValue(101, "Dr. Who")
        private val staffDrScott = IdValue(102, "Dr. Scott")

        private var startDateAppointment1 = tomorrowDate.withHour(14).withMinute(0).format(dateFormatter)
        private var endDateAppointment1 = tomorrowDate.withHour(14).withMinute(10).format(dateFormatter)

        private var startDateAppointment2 = tomorrowDate.withHour(15).withMinute(20).format(dateFormatter)
        private var endDateAppointment2 = tomorrowDate.withHour(15).withMinute(30).format(dateFormatter)

        fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
            return AppointmentsSlotsExampleBuilder()
                    .appointmentSessions(arrayListOf(AppointmentSessionFacadeBuilder()
                            .sessionId(firstSessionId)
                            .sessionType(clinic)
                            .staffDetails(arrayListOf(staffDrWho))
                            .location(locationLeeds)
                            .slots {
                                addAppointment {
                                    slotId(firstSlotId)
                                            .startDate(startDateAppointment1)
                                            .endDate(endDateAppointment1)
                                }
                            }.build(),
                            AppointmentSessionFacadeBuilder()
                                    .sessionId(secondSessionId)
                                    .sessionType(clinic)
                                    .staffDetails(arrayListOf(staffDrScott))
                                    .location(locationLeeds)
                                    .slots {
                                        addAppointment {
                                            slotId(secondSlotId)
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
                            ), SlotResponseObject(
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
