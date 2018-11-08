package features.appointments.factories

import com.github.tomakehurst.wiremock.stubbing.Scenario
import constants.DateTimeFormats
import features.sharedSteps.SupplierSpecificFactory
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.time.Duration
import java.util.*

private const val DELAY_IN_SECONDS = 90L

abstract class UpcomingAppointmentsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    private val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    private val baseDate = Calendar.getInstance(timeZone)
    private val appointmentsFromDate = gpDateTimeFormat.format(baseDate.time)

    fun createSuccessfulEmptyUpcomingAppointmentResponse(
            cancellationReasons: List<AppointmentCancellationReason> = getDefaultCancellationReasons()
    ) {
        val facade = MyAppointmentsFacade(
                appointmentsFromDate,
                AppointmentSlotsResponseFacade(
                        cancellationReasons = cancellationReasons,
                        bookingReasonNecessityOption = getBookingReasonNecessity()
                )
        )
        mockUpcomingAppointments {
            respondWithSuccess(facade)
                    .inScenario("Appointments")
                    .whenScenarioStateIs(Scenario.STARTED)
        }
        Serenity.setSessionVariable(MyAppointmentsFacade::class).to(facade)
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfSlots(facade))
    }

    fun createSuccessfulUpcomingAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade
            = AppointmentsSlotsExample.getGenericExample(),
            numberOfCancellationReasons: Int = getDefaultCancellationReasons().size
    ) {
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons().subList(
                0,
                numberOfCancellationReasons
        )

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createUpcomingAppointments {
            respondWithSuccess(myAppointmentsFacade)
        }
        Serenity.setSessionVariable(MyAppointmentsFacade::class).to(myAppointmentsFacade)
        Serenity.setSessionVariable(Expectations.EXPECTED_API_RESPONSE_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedApiResponse(myAppointmentsFacade.slots!!.sessions))
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfSlots(myAppointmentsFacade))
    }

    fun createSuccessfulUpcomingAppointmentsResponseOnceBooked(
            numberOfCancellationReasons: Int = getDefaultCancellationReasons().size
    ) {
        val sessionOfSelectedSlot = Serenity.sessionVariableCalled<List<AppointmentSessionFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .APPOINTMENT_SLOTS_EXAMPLE_SESSIONS
        ).first()
        val sessionWithOnlySelectedSlot = sessionOfSelectedSlot.copy(
                slots = arrayListOf(sessionOfSelectedSlot.slots.first())
        )
        val appointmentSlotsResponseFacade = AppointmentSlotsResponseFacade(arrayListOf(sessionWithOnlySelectedSlot))
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons().subList(
                0,
                numberOfCancellationReasons
        )

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createUpcomingAppointments {
            respondWithSuccess(myAppointmentsFacade)
                    .inScenario("Appointments")
                    .whenScenarioStateIs("Appointment Booked")
        }

        Serenity.setSessionVariable(MyAppointmentsFacade::class).to(myAppointmentsFacade)
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfSlots(myAppointmentsFacade))
    }

    fun createCorruptedUpcomingAppointmentsResponse() {
        createUpcomingAppointments {
            respondWithCorrupted()
        }
    }

    fun createTimeoutUpcomingAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade = AppointmentsSlotsExample
                    .getGenericExample()
    ) {
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons()

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createUpcomingAppointments {
            respondWithSuccess(myAppointmentsFacade).delayedBy(Duration.ofSeconds(DELAY_IN_SECONDS))
        }
    }

    fun createUpcomingAppointments(mapping: (IMyAppointmentsBuilder.() -> Mapping)) {
        mockUpcomingAppointments(mapping)
    }


    private fun getBookingReasonNecessity(): NecessityOption {
        return SerenityHelpers.getValueOrNull<NecessityOption>("BookingReasonNecessity") ?: NecessityOption.NOT_ALLOWED
    }

    private fun convertToMyAppointmentsFacade(facade: AppointmentSlotsResponseFacade): MyAppointmentsFacade {
        return MyAppointmentsFacade(
                appointmentsFromDate,
                facade
        )
    }

    private fun createBackendDateTimeFormatWithoutTimezone(): SimpleDateFormat {
        val sdf = SimpleDateFormat(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        sdf.timeZone = timeZone
        return sdf
    }

    private fun mockUpcomingAppointments(mapping: (IMyAppointmentsBuilder.() -> Mapping)) {
        appointmentMapper.requestMapping { mapping(viewMyAppointmentsRequest(patient)) }
    }

    protected fun slotDateFormat(dateTimeToConvert: Date): String {
        val slotDateFormat = SimpleDateFormat(DateTimeFormats.frontendDateFormat)
        slotDateFormat.timeZone = timeZone
        return slotDateFormat.format(dateTimeToConvert)
    }

    protected fun slotTimeFormat(dateTimeToConvert: Date): String {
        val slotTimeFormat = SimpleDateFormat(DateTimeFormats.frontendTimeFormat)
        slotTimeFormat.timeZone = timeZone
        return slotTimeFormat.format(dateTimeToConvert).toLowerCase()
    }

    abstract fun getExpectedApiResponse(facade: ArrayList<AppointmentSessionFacade>): MyAppointmentsResponse

    abstract fun getExpectedUiRepresentationOfSlots(facade: MyAppointmentsFacade): List<Slot>

    abstract fun filterUpcomingAppointmentsWhenAppropriate(facade: ArrayList<AppointmentSessionFacade>):
            List<AppointmentSessionFacade>

    abstract fun getDefaultCancellationReasons(): List<AppointmentCancellationReason>

    companion object : SupplierSpecificFactory<UpcomingAppointmentsFactory>() {

        override val map: HashMap<String, (() -> (UpcomingAppointmentsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { UpcomingAppointmentsFactoryEmis() },
                    "TPP" to { UpcomingAppointmentsFactoryTpp() },
                    "VISION" to { UpcomingAppointmentsFactoryVision() }
            )
        }
    }

    enum class Expectations {
        EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS,
        EXPECTED_API_RESPONSE_OF_MY_UPCOMING_APPOINTMENTS
    }

}
