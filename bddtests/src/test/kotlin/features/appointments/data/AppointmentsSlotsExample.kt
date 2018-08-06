package features.appointments.data

import features.appointments.factories.AppointmentSessionFacadeBuilder
import joptsimple.util.KeyValuePair
import mockingFacade.appointments.AppointmentFilterFacade
import worker.models.appointments.SlotResponseObject
import java.util.*

class AppointmentsSlotsExample: AppointmentsSlotsExampleBase() {

    private var startDateAppointment1 = tomorrowDate.withHour(14).withMinute(0).format(dateFormatter)
    private var endDateAppointment1 = tomorrowDate.withHour(14).withMinute(10).format(dateFormatter)

    private var startDateAppointment2 = tomorrowDate.withHour(15).withMinute(20).format(dateFormatter)
    private var endDateAppointment2 = tomorrowDate.withHour(15).withMinute(30).format(dateFormatter)

    private val session1 = AppointmentSessionFacadeBuilder()
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
            }.build()

    private val session2 = AppointmentSessionFacadeBuilder()
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
            }.build()

    override val appointmentSessions = arrayListOf(session1, session2)

    override val filter =
            AppointmentFilterFacade(
                    type = clinicSlot,
                    doctor = staffDrWho.value,
                    location = locationLeeds.value
            )

    override val expectedResponseSlots = arrayListOf(
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

    override var appointmentTypesList: ArrayList<String> = arrayListOf(clinicSlot)
    override var locationsList: ArrayList<String> = arrayListOf(locationLeeds.value, locationSheffield.value)
    override var cliniciansList: ArrayList<String> = arrayListOf(staffDrWho.value, staffDrScott.value)
}