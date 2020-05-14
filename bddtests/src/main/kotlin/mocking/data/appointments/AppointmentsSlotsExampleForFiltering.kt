package mocking.data.appointments

import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
import models.AppointmentDate

class AppointmentsSlotsExampleForFiltering : AppointmentsSlotsExample() {

    private var nextAppointmentDateToSet = currentTime.plusHours(HOURLY_INCREMENT.toLong())

    private val generateAppointmentData = GenerateAppointmentData()

    fun getExampleForFilteringSlotTypesAndLocations(): AppointmentSlotsResponseFacade {
        val appointmentSlotsResponseFacadeBuilder = generateAppointmentSlotsResponseFacadeBuilderWithAllCriteria()

        val filter = generateAppointmentData.generateFilter(
                DEFAULT_SLOT_TYPE,
                locationLeeds.locationName,
                null,
                appointmentSlotsResponseFacadeBuilder.build()
        )

        return appointmentSlotsResponseFacadeBuilder
                .filterValues(filter)
                .build()
    }

    fun getExampleForFilteringClinincian(): AppointmentSlotsResponseFacade {
        val appointmentSlotsResponseFacadeBuilder = generateAppointmentSlotsResponseFacadeBuilderWithAllCriteria()

        val filter = generateAppointmentData.generateFilter(
                DEFAULT_SLOT_TYPE,
                locationLeeds.locationName,
                staffDrWho.staffName,
                appointmentSlotsResponseFacadeBuilder.build()
        )

        return appointmentSlotsResponseFacadeBuilder
                .filterValues(filter)
                .build()
    }

    private fun generateAppointmentSlotsResponseFacadeBuilderWithAllCriteria():
            AppointmentsSlotsExampleBuilder {

        val slotTypeAlternate = SlotTypeFacade(nextSlotTypeId++, "Bloods")
        val appointmentSessions = ArrayList(generateAppointmentSessionsForSessionNames(slotTypeAlternate))

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .locationsList(listOf(locationLeeds, locationSheffield))
                .appointmentTypesList(listOf(slotTypeDefault, slotTypeAlternate))
                .cliniciansList(listOf(staffDrWho, staffDrScott))
                .appointmentSessions(appointmentSessions)
    }

    private fun generateAppointmentSessionsForSessionNames(
            slotTypeAlternate: SlotTypeFacade
    ): List<AppointmentSessionFacade> {

        return arrayListOf(CLINIC_SESSION_TYPE, TELEPHONE_SESSION_TYPE).flatMap { sessionType ->
            generateAppointmentSessionsForLocations(slotTypeAlternate, sessionType)
        }
    }

    private fun generateAppointmentSessionsForLocations(
            slotTypeAlternate: SlotTypeFacade, sessionType: String
    ): List<AppointmentSessionFacade> {

        return arrayListOf(locationLeeds, locationSheffield).flatMap { location ->
            generateAppointmentSessionsForClinicians(slotTypeAlternate, sessionType, location)
        }
    }

    private fun generateAppointmentSessionsForClinicians(
            slotTypeAlternate: SlotTypeFacade,
            sessionType: String,
            location: LocationFacade
    ): List<AppointmentSessionFacade> {

        return arrayListOf(staffDrWho, staffDrScott).flatMap { clinician ->
            generateAppointmentSessionsForSlotTypes(slotTypeAlternate, sessionType, location, clinician)
        }
    }

    private fun generateAppointmentSessionsForSlotTypes(
            slotTypeAlternate: SlotTypeFacade,
            sessionType: String,
            location: LocationFacade,
            clinician: StaffDetailsFacade
    ): List<AppointmentSessionFacade> {

        return arrayListOf(slotTypeDefault, slotTypeAlternate).map { slotType ->
            generateAppointmentSessionFacade(sessionType, location, clinician, slotType)
        }
    }

    private fun generateAppointmentSessionFacade(
            sessionType: String,
            location: LocationFacade,
            clinician: StaffDetailsFacade,
            slotType: SlotTypeFacade): AppointmentSessionFacade {

        return generateAppointmentData.generateAppointmentSession(
                AppointmentSessionFacadeBuilder()
                        .sessionType(sessionType)
                        .locationId(location.locationId)
                        .staffDetails(clinician.staffDetailsid),
                slotTypes = arrayListOf(slotType),
                dates = generateAppointmentDates(1) as ArrayList<AppointmentDate>,
                staff = clinician
        )
    }

    private fun generateAppointmentDates(numberOfAppointments: Int): List<AppointmentDate> {
        val createdAppointmentDates: ArrayList<AppointmentDate> = ArrayList()
        (1..numberOfAppointments).forEach { _ ->
            createdAppointmentDates.add(AppointmentDate(nextAppointmentDateToSet))
            nextAppointmentDateToSet = nextAppointmentDateToSet.plusHours(HOURLY_INCREMENT.toLong())
        }
        return createdAppointmentDates
    }
}
