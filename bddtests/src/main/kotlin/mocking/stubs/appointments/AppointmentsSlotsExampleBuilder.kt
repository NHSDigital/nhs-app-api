package mocking.stubs.appointments

import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.util.*

open class AppointmentsSlotsExampleBuilder {

    protected var appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
    protected var filter: AppointmentFilterFacade = AppointmentFilterFacade()
    protected var appointmentTypesList: ArrayList<String> = arrayListOf()
    protected var locationsList: ArrayList<String> = arrayListOf()
    protected var cliniciansList: ArrayList<String> = arrayListOf()

    fun appointmentSessions(value: ArrayList<AppointmentSessionFacade>): AppointmentsSlotsExampleBuilder {
        appointmentSessions = value
        return this
    }

    fun filterValues(value: AppointmentFilterFacade): AppointmentsSlotsExampleBuilder {
        filter = value
        return this
    }

    fun appointmentTypesList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        appointmentTypesList = value
        return this
    }

    fun locationsList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        locationsList = value
        return this
    }

    fun cliniciansList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        cliniciansList = value
        return this
    }

    open fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {
        return AppointmentSlotsResponseFacade(appointmentSessions, "1")
    }
}
