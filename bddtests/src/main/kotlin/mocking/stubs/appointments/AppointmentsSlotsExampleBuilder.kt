package mocking.stubs.appointments

import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.util.*

open class AppointmentsSlotsExampleBuilder {

    protected var appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
    protected var filter: AppointmentFilterFacade = AppointmentFilterFacade()
    protected var appointmentTypesList: List<String> = listOf()
    protected var locationsList: List<String> = listOf()
    protected var cliniciansList: List<String> = listOf()

    fun appointmentSessions(value: ArrayList<AppointmentSessionFacade>): AppointmentsSlotsExampleBuilder {
        appointmentSessions = value
        appointmentTypesList = appointmentTypesList(value)
        locationsList = locationsList(value)
        cliniciansList = cliniciansList(value)
        return this
    }

    fun filterValues(value: AppointmentFilterFacade): AppointmentsSlotsExampleBuilder {
        filter = value
        return this
    }

    private fun appointmentTypesList(value: ArrayList<AppointmentSessionFacade>): List<String> {
        return value.flatMap { session ->
            session.slots.map { slot ->
                slot.slotTypeName!!
            }
        }.distinct()
    }

    private fun locationsList(value: ArrayList<AppointmentSessionFacade>): List<String> {
        return value.map { session ->
            session.location!!
        }.distinct()
    }

    private fun cliniciansList(value: ArrayList<AppointmentSessionFacade>): List<String> {
        return value.flatMap { session ->
            session.staffDetails.map { staff ->
                staff.staffName!!
            }
        }.distinct()
    }

    open fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {
        return AppointmentSlotsResponseFacade(appointmentSessions, "1")
    }
}
