package mocking.data.appointments

import constants.DateTimeFormats
import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mocking.stubs.appointments.AppointmentSlotFacadeArrayBuilder
import mocking.stubs.appointments.AppointmentSlotFacadeBuilder
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
import models.AppointmentDate
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

private const val STARTING_SESSION_ID = 0
private const val STARTING_SLOT_ID = 100

class GenerateAppointmentData {

    private var sessionClinicType = "Clinic"

    var sessionId = STARTING_SESSION_ID
    var slotId = STARTING_SLOT_ID

    fun generateAppointments(locationNames: List<LocationFacade>,
                             typesNames: List<SlotTypeFacade>,
                             staffNames: List<StaffDetailsFacade>,
                             dates: ArrayList<AppointmentDate>): ArrayList<AppointmentSessionFacade> {

        val appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()

        for (staff in staffNames) {
            for (location in locationNames) {
                appointmentSessions.add(
                        generateAppointmentSession(
                                AppointmentSessionFacadeBuilder()
                                        .sessionType(sessionClinicType)
                                        .locationId(location.locationId)
                                        .staffDetails(staff.staffDetailsid),
                                staff,
                                typesNames,
                                dates
                        )
                )
            }
        }

        return appointmentSessions
    }

    fun generateAppointmentSession(sessionDetails: AppointmentSessionFacadeBuilder,
                                   staff: StaffDetailsFacade,
                                   slotTypes: List<SlotTypeFacade>,
                                   dates: ArrayList<AppointmentDate>,
                                   telephoneNumber: String = ""):
            AppointmentSessionFacade {

        val appointmentSessionFacade = sessionDetails.build()
        val sessionType = appointmentSessionFacade.sessionType

        return sessionDetails
                .sessionId(sessionId++)
                .slots { generateSlotsForAppointment(dates, slotTypes, staff, sessionType!!, telephoneNumber) }
                .build()
    }

    fun generateStaffDetails(staff: ArrayList<String>): List<StaffDetailsFacade> {
        val staffDetails: ArrayList<StaffDetailsFacade> = arrayListOf()
        for (staffName in staff) {
            staffDetails.add(StaffDetailsFacade(staff.indexOf(staffName), staffName))
        }
        return staffDetails
    }

    fun generateLocationDetails(locations: ArrayList<String>): List<LocationFacade> {
        val locationDetails: ArrayList<LocationFacade> = arrayListOf()

        for (location in locations) {
            locationDetails.add(LocationFacade(locations.indexOf(location), location))
        }

        return locationDetails
    }

    fun generateSlotTypes(typesNames: ArrayList<String>): List<SlotTypeFacade> {
        val slotTypes: ArrayList<SlotTypeFacade> = arrayListOf()

        for (typeName in typesNames) {
            slotTypes.add(SlotTypeFacade(typesNames.indexOf(typeName), typeName))
        }

        return slotTypes
    }

    private fun generateSlotsForAppointment(
            dates: List<AppointmentDate>,
            types: List<SlotTypeFacade>,
            staff: StaffDetailsFacade,
            sessionType: String,
            telephoneNumber: String
    ):
            AppointmentSlotFacadeArrayBuilder {

        // get telephone number better
        val appointmentSlotFacadeArrayBuilder = AppointmentSlotFacadeArrayBuilder()

        for (date in dates) {
            val isSlotInPast = date.date < LocalDateTime.now()

            val startDate = FilterSlotDetails(date.date, date.hour, date.minute)
            val endDate = FilterSlotDetails(startDate.dateAsLocalDateTime.plusMinutes(date.duration.toLong()))
            val channel = if (telephoneNumber != "") SlotTypeStatus.Telephone else SlotTypeStatus.Unknown

            for (type in types) {
                val appointment = AppointmentSlotFacadeBuilder()
                        .slotId(slotId++)
                        .slotTypeId(type.slotTypeId)
                        .startDate(startDate.dateTimeAsBackendString)
                        .endDate(endDate.dateTimeAsBackendString)
                        .channel(channel)
                        .setSlotInThePast(isSlotInPast)
                        .slotDetails(type.slotTypeName, sessionType, staff.staffName)
                        .telephoneNumber(telephoneNumber)

                appointmentSlotFacadeArrayBuilder.addAppointment { appointment }
            }
        }

        return appointmentSlotFacadeArrayBuilder
    }

    fun generateFilter(
            type: String,
            location: String? = null,
            doctor: String? = null,
            appointmentsSlotsResponse: AppointmentSlotsResponseFacade
    ): AppointmentFilterFacade {

        val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)

        fun getAssociatedSessionLocationName(session: AppointmentSessionFacade): String {
            return appointmentsSlotsResponse.locations.find { location ->
                location.locationId == session.locationId
            }!!.locationName
        }

        fun hasAssociatedClinician(session: AppointmentSessionFacade): Boolean {
            return appointmentsSlotsResponse.staffDetails.any { staffDetails ->
                session.staffDetails.any { staffId ->
                    staffId == staffDetails.staffDetailsid
                            && staffDetails.staffName == doctor
                }
            }
        }

        fun convertToSlotDetails(session: AppointmentSessionFacade, slot: AppointmentSlotFacade): FilterSlotDetails {
            return FilterSlotDetails(
                    LocalDateTime.parse(slot.startTime, currentDateFormat)
            ).sessionName(session.sessionType!!)
        }

        fun getAssociatedAppointmentSlot(session: AppointmentSessionFacade): List<AppointmentSlotFacade> {
            return session.slots.filter { slot ->
                type == appointmentsSlotsResponse.slotTypes.find { slotType ->
                    slotType.slotTypeId == slot.slotTypeId
                }!!.slotTypeName
            }
        }

        val listOfFilteredSlots = appointmentsSlotsResponse.sessions
                .filter { session ->
                    (location == null || location == getAssociatedSessionLocationName(session))
                            && (doctor == null || hasAssociatedClinician(session))
                }
                .flatMap { session ->
                    getAssociatedAppointmentSlot(session)
                            .map { slot ->
                                convertToSlotDetails(session, slot)
                            }
                }

        return AppointmentFilterFacade(
                type,
                location,
                doctor,
                generateMapOfAppointmentDatesAndTimes(listOfFilteredSlots)
        )
    }

    private fun generateMapOfAppointmentDatesAndTimes(listOfFilteredSlots: List<FilterSlotDetails>)
            : Map<String, Set<FilterSlotDetails>> {
        var result = mapOf<String, Set<FilterSlotDetails>>()
        for (slotDetails in listOfFilteredSlots) {
            val date = slotDetails.dateAsUIString
            val currentSetOfSlots = result[date] ?: setOf()
            result = result.plus(Pair(date, currentSetOfSlots.plus(slotDetails)))
        }
        return result
    }
}
