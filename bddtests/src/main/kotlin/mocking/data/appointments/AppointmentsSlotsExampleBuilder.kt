package mocking.data.appointments

import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
import java.util.*

open class AppointmentsSlotsExampleBuilder {

    protected var appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
    protected var appointmentTypesList: List<SlotTypeFacade> = listOf()
    protected var locationsList: List<LocationFacade> = listOf()
    protected var cliniciansList: List<StaffDetailsFacade> = listOf()
    protected var filter: AppointmentFilterFacade = AppointmentFilterFacade()

    fun appointmentSessions(value: ArrayList<AppointmentSessionFacade>): AppointmentsSlotsExampleBuilder {
        appointmentSessions = value
        return this
    }

    fun filterValues(value: AppointmentFilterFacade): AppointmentsSlotsExampleBuilder {
        filter = value
        return this
    }

    fun appointmentTypesList(value: List<SlotTypeFacade>): AppointmentsSlotsExampleBuilder {
        appointmentTypesList = value
        return this
    }

    fun locationsList(value: List<LocationFacade>): AppointmentsSlotsExampleBuilder {
        locationsList = value
        return this
    }

    fun cliniciansList(value: List<StaffDetailsFacade>): AppointmentsSlotsExampleBuilder {
        cliniciansList = value
        return this
    }

    open fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {
        return AppointmentSlotsResponseFacade(
                appointmentSessions,
                "1",
                locationsList,
                cliniciansList,
                appointmentTypesList
        )
    }
}
