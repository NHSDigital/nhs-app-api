package features.appointments.factories

import constants.DateTimeFormats
import features.appointments.data.AppointmentsSlotsExample
import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.time.Duration
import java.util.*
import kotlin.collections.ArrayList

abstract class UpcomingAppointmentsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    private val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    private val baseDate = Calendar.getInstance(timeZone)
    private val appointmentsFromDate = gpDateTimeFormat.format(baseDate.time)

    fun createSuccessfulEmptyUpcomingAppointmentResponse() {
        val facade = MyAppointmentsFacade(appointmentsFromDate)
        mockUpcomingAppointments {
            respondWithSuccess(facade)
        }
        Serenity.setSessionVariable(MyAppointmentsFacade::class).to(facade)
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfSlots(facade))
    }

    fun createSuccessfulUpcomingAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade
            = AppointmentsSlotsExample.getGenericExample()
    ) {
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

    fun createCorruptedUpcomingAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade
            = AppointmentsSlotsExample.getGenericExample()
    ) {
        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createUpcomingAppointments {
            respondWithCorrupted(myAppointmentsFacade)
        }
    }

    fun createTimeoutUpcomingAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade = AppointmentsSlotsExample
            .getGenericExample()
    ) {
        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createUpcomingAppointments {
            respondWithSuccess(myAppointmentsFacade).delayedBy(Duration.ofSeconds(90))
        }
    }

    fun createUpcomingAppointments(mapping: (IMyAppointmentsBuilder.() -> Mapping)) {
        mockUpcomingAppointments(mapping)
        setCancellationReasons()
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

    abstract fun setCancellationReasons()

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