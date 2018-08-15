package features.appointments.stepDefinitions.factories

import constants.AppointmentDateTimeFormat
import features.appointments.data.ViewAppointmentsFactory
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.*
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import java.time.Duration
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*

abstract class AppointmentsBookingFactory(gpSupplier:String): AppointmentsFactory(gpSupplier) {

    private var dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)

    private var tomorrowDate = LocalDateTime.now().plusDays(1)

    protected var startDateAppointment1 = tomorrowDate.withHour(14).withMinute(0).format(dateFormatter)
    protected var endDateAppointment1 =  tomorrowDate.withHour(14).withMinute(10).format(dateFormatter)

    protected var startDateAppointment2 =   tomorrowDate.withHour(15).withMinute(20).format(dateFormatter)
    protected var endDateAppointment2 =  tomorrowDate.withHour(15).withMinute(30).format(dateFormatter)

    val defaultAppointmentSlots = arrayListOf(
            AppointmentSlotFacade(
                    slotId = 301,
                    startTime = startDateAppointment1,
                    endTime = endDateAppointment1,
                    slotTypeName = "Slot"
            ),
            AppointmentSlotFacade(
                    slotId = 302,
                    startTime = startDateAppointment2,
                    endTime = endDateAppointment2,
                    slotTypeName = "Slot"
            )
    )

    val defaultAppointmentSessions = arrayListOf(
            AppointmentSessionFacade(
                    sessionId = 301,
                    sessionType = "Clinic",
                    staffDetails = "Dr. Who",
                    location = "Leeds",
                    slots = defaultAppointmentSlots
            ),
            AppointmentSessionFacade(
                    sessionId = 302,
                    sessionType = "Clinic",
                    staffDetails = "Dr. Scott",
                    location = "Sheffield",
                    slots = defaultAppointmentSlots
            )
    )

    val defaultFilter =
            AppointmentFilterFacade(
                    type = "Clinic - Slot",
                    doctor = "Dr. Who",
                    location = "Leeds"
            )

    fun generateDefaultAvailableAppointmentSlotExample() {
        generateDefaultUserData()
        generateDefaultAppointmentSlots()
        Serenity.setSessionVariable(ExpectedAppointmentFilterFacadeKey).to(defaultFilter)

        //Format like : Wednesday 1 August 2018
        val formatter = DateTimeFormatter.ofPattern("EEEE d MMMM yyyy")
        val day = tomorrowDate.format(formatter)
        Serenity.setSessionVariable(TargetAppointmentDateKey).to(day)
        Serenity.setSessionVariable(TargetAppointmentTimeKey).to("2:00 pm")
    }

    private fun generateDefaultUserData() {

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        createGetEmptyAppointmentList()
        Serenity.setSessionVariable(ExpectedAppointmentTypesKey).to(appointmentTypesList)
        Serenity.setSessionVariable(ExpectedAppointmentLocationsKey).to(locationsList)
        Serenity.setSessionVariable(ExpectedAppointmentCliniciansKey).to(cliniciansList)
    }

    protected abstract fun generateDefaultAppointmentSlots()

    private var appointmentTypesList: ArrayList<String> =arrayListOf("Clinic - Slot")
    private var locationsList : ArrayList<String> =arrayListOf("Leeds", "Sheffield")
    private var cliniciansList : ArrayList<String> =arrayListOf("Dr. Who", "Dr. Scott")

    fun generateSuccessfulBookingResponse() {
        generateBookingResponse{bookRequest->bookRequest.respondWithSuccess()}
    }

    fun generateBookingResponse(booker: (IBookAppointmentsBuilder) -> Mapping) {
        appointmentMapper.requestMapping {
            booker(bookAppointmentSlotRequest(patient,
                    BookAppointmentSlotFacade(patient.userPatientLinkToken, 301, "Reason"))
            )
        }
    }

    private fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(supplier)
        val getResponse = viewAppointmentFactory.createEmptyUpcomingAppointmentResponse(patient)
        appointmentMapper
                .requestMapping{ viewMyAppointmentsRequest(patient).respondWithSuccess(getResponse)}
    }

    protected fun generateAppointmentSlotResponse(patient: Patient,
                                                  appointmentSlots:AppointmentSlotsResponseFacade,
                                                  delayedInSeconds: Long){
        appointmentMapper.requestMapping {
            appointmentSlotsRequest(patient).respondWithSuccess(appointmentSlots)
                    .delayedBy(Duration.ofSeconds(delayedInSeconds)) }
    }


    companion object {

        private val map : HashMap<String, (() -> (AppointmentsBookingFactory))> by lazy {
                    hashMapOf(
                            "EMIS" to { AppointmentsBookingFactoryEmis() },
                            "TPP" to { AppointmentsBookingFactoryTpp() })
        }

        fun getForSupplier(gpSystem: String): AppointmentsBookingFactory {
            if (!map.containsKey(gpSystem)) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem).invoke()
        }


        const val ExpectedAppointmentTypesKey = "ExpectedAppointmentTypesKey"
        const val  ExpectedAppointmentLocationsKey = "ExpectedAppointmentLocationsKey"
        const val  ExpectedAppointmentCliniciansKey= "ExpectedAppointmentCliniciansKey"
        const val  ExpectedAppointmentFilterFacadeKey = "ExpectedAppointmentFilterFacadeKey"

        const val  TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val  TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}