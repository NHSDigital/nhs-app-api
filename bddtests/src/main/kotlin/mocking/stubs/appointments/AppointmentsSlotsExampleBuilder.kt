package mocking.stubs.appointments

import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import worker.models.appointments.SlotResponseObject
import java.util.*

class AppointmentsSlotsExampleBuilder {

    private var appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
    private var filter: AppointmentFilterFacade = AppointmentFilterFacade()
    private var expectedResponseSlots: ArrayList<SlotResponseObject> = arrayListOf()
    private var appointmentTypesList: ArrayList<String> = arrayListOf()
    private var locationsList: ArrayList<String> = arrayListOf()
    private var cliniciansList: ArrayList<String> = arrayListOf()

    fun appointmentSessions(value: ArrayList<AppointmentSessionFacade>): AppointmentsSlotsExampleBuilder {
        appointmentSessions = value
        return this
    }

    fun filterValues(value: AppointmentFilterFacade): AppointmentsSlotsExampleBuilder {
        filter = value
        return this
    }

    fun expectedResponseSlots(value: ArrayList<SlotResponseObject>): AppointmentsSlotsExampleBuilder {
        expectedResponseSlots = value
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

    fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {

        val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(appointmentSessions, "1")
        setExpectations(appointmentSessions)
        return getAppointmentSlotsResponseModel
    }


    private fun setExpectations(appointmentSessions: ArrayList<AppointmentSessionFacade>) {
        val appointmentSlots = arrayListOf<AppointmentSlotFacade>()
        appointmentSlots.addAll(appointmentSessions.flatMap { session -> session.slots })

    }


}