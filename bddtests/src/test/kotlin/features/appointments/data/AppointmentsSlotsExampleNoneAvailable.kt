package features.appointments.data

import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import worker.models.appointments.SlotResponseObject
import java.util.*

class AppointmentsSlotsExampleNoneAvailable: AppointmentsSlotsExampleBase() {

    override val appointmentSessions = arrayListOf<AppointmentSessionFacade>()

    override val filter = AppointmentFilterFacade(type = "", doctor = "", location = "")

    override val expectedResponseSlots = arrayListOf<SlotResponseObject>()

    override var appointmentTypesList: ArrayList<String> = arrayListOf()

    override var locationsList: ArrayList<String> = arrayListOf()

    override var cliniciansList: ArrayList<String> = arrayListOf()
}