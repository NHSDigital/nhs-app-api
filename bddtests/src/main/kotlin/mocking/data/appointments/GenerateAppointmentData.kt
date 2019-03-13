package mocking.data.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mocking.stubs.appointments.AppointmentSlotFacadeArrayBuilder
import mocking.stubs.appointments.AppointmentSlotFacadeBuilder
import mocking.stubs.appointments.IdValue
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.AppointmentDate
import java.time.LocalDateTime
import kotlin.collections.ArrayList

class GenerateAppointmentData {

    private var sessionClinicType = "Clinic"

    companion object {
        var sessionId = 0
        var slotId = 100
    }


    fun generateAppointments(locationNames: ArrayList<String>,
                             typesArray: ArrayList<String>,
                             staffNames: ArrayList<String>,
                             dates: ArrayList<AppointmentDate>,
                             filter: AppointmentFilterFacade): AppointmentSlotsResponseFacade {

        val appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
        val staffDetails = generateStaffDetails(staffNames)
        val locationDetails = generateLocationDetails(locationNames)

        for (staff in staffDetails) {
            for (location in locationDetails) {
                appointmentSessions.add(
                        generateAppointmentSession(
                                sessionClinicType, location, staff, dates)
                )
            }
        }

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .appointmentTypesList(typesArray)
                .locationsList(locationNames)
                .cliniciansList(staffNames)
                .filterValues(filter)
                .build()

    }

    fun generateAppointmentSession(sessionType: String,
                                   location: IdValue, staffDetails: IdValue,
                                   dates: ArrayList<AppointmentDate>,
                                   channel: SlotTypeStatus = SlotTypeStatus.Unknown):
            AppointmentSessionFacade {


        return AppointmentSessionFacadeBuilder()
                .sessionId(sessionId++)
                .sessionType(sessionType)
                .location(location)
                .staffDetails(staffDetails)
                .slots { generateSlotsForAppointment(dates, channel) }
                .build()
    }

    private fun generateStaffDetails(staff: ArrayList<String>): ArrayList<IdValue> {
        val staffDetails: ArrayList<IdValue> = arrayListOf()
        for (staffName in staff) {
            staffDetails.add(IdValue(staff.indexOf(staffName), staffName))
        }
        return staffDetails
    }

    private fun generateLocationDetails(locations: ArrayList<String>): ArrayList<IdValue> {
        val locationDetails: ArrayList<IdValue> = arrayListOf()

        for (location in locations) {
            locationDetails.add(IdValue(locations.indexOf(location), location))
        }

        return locationDetails
    }


    private fun generateSlotsForAppointment(dates: ArrayList<AppointmentDate>, channel: SlotTypeStatus):
            AppointmentSlotFacadeArrayBuilder {

        val appointmentSlotFacadeArrayBuilder = AppointmentSlotFacadeArrayBuilder()


        for (date in dates) {
            var isSlotInPast = false
            val now = LocalDateTime.now()

            if (date.date < now) {
                isSlotInPast = true
            }

            val startDate = FilterSlotDetails(date.date, date.hour, date.minute)
            val endDate = FilterSlotDetails(date.date, date.hour, date.minute.plus(date.duration))

            val appointment = AppointmentSlotFacadeBuilder()
                    .slotId(slotId++)
                    .startDate(startDate.dateTimeAsBackendString)
                    .endDate(endDate.dateTimeAsBackendString)
                    .channel(channel)
                    .setSlotInThePast(isSlotInPast)



            appointmentSlotFacadeArrayBuilder.addAppointment { appointment }

        }

        return appointmentSlotFacadeArrayBuilder
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


    fun generateFilter(type: String? = null, doctor: String? = null, location: String? = null,
                       dateArray: ArrayList<AppointmentDate>): AppointmentFilterFacade {

        val dates: ArrayList<FilterSlotDetails> = arrayListOf()

        for (aDate in dateArray) {
            val date = FilterSlotDetails(aDate.date, aDate.hour, aDate.minute).sessionName(aDate.sessionName)
            dates.add(date)
        }

        return AppointmentFilterFacade(
                type = type,
                doctor = doctor,
                location = location,
                filteredSlots = generateMapOfAppointmentDatesAndTimes(dates)
        )
    }

}