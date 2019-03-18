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

private const val STARTING_SESSION_ID = 0
private const val STARTING_SLOT_ID = 100

class GenerateAppointmentData {

    private var sessionClinicType = "Clinic"

    var sessionId = STARTING_SESSION_ID
    var slotId = STARTING_SLOT_ID

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
                                AppointmentSessionFacadeBuilder()
                                        .sessionType(sessionClinicType)
                                        .location(location)
                                        .staffDetails(staff),
                                typesArray,
                                dates
                        )
                )
            }
        }

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
                .build()

    }

    fun generateAppointmentSession(sessionDetails: AppointmentSessionFacadeBuilder,
                                   slotTypes: ArrayList<String>,
                                   dates: ArrayList<AppointmentDate>,
                                   channel: SlotTypeStatus = SlotTypeStatus.Unknown):
            AppointmentSessionFacade {

        return sessionDetails
                .sessionId(sessionId++)
                .slots { generateSlotsForAppointment(dates, slotTypes, channel) }
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


    private fun generateSlotsForAppointment(dates: ArrayList<AppointmentDate>, types: ArrayList<String>, channel:
    SlotTypeStatus):
            AppointmentSlotFacadeArrayBuilder {

        val appointmentSlotFacadeArrayBuilder = AppointmentSlotFacadeArrayBuilder()


        for (date in dates) {
            val isSlotInPast = date.date < LocalDateTime.now()

            val startDate = FilterSlotDetails(date.date, date.hour, date.minute)
            val endDate = FilterSlotDetails(date.date, date.hour, date.minute?.plus(date.duration))

            for (type in types) {
                val appointment = AppointmentSlotFacadeBuilder()
                        .slotId(slotId++)
                        .slotTypeName(type)
                        .startDate(startDate.dateTimeAsBackendString)
                        .endDate(endDate.dateTimeAsBackendString)
                        .channel(channel)
                        .setSlotInThePast(isSlotInPast)

                appointmentSlotFacadeArrayBuilder.addAppointment { appointment }
            }
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
