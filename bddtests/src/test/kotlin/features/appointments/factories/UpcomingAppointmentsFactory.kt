package features.appointments.factories

import constants.DateTimeFormats
import features.appointments.data.AppointmentsSlotsExample
import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import java.text.SimpleDateFormat
import java.time.Duration
import java.util.*

abstract class UpcomingAppointmentsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    private val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val dateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    private val baseDate = Calendar.getInstance(timeZone)
    private val appointmentsFromDate = dateTimeFormat.format(baseDate.time)

    fun createSuccessfulEmptyUpcomingAppointmentResponse() {
        mockUpcomingAppointments {
            respondWithSuccess(
                    MyAppointmentsFacade(appointmentsFromDate)
            )
        }
    }

    fun createSuccessfulUpcomingAppointmentsResponse(facade: MyAppointmentsFacade = genericMyAppointmentsFacade()) {
        createUpcomingAppointments {
            respondWithSuccess(facade)
        }
    }

    fun createCorruptedUpcomingAppointmentsResponse(facade: MyAppointmentsFacade = genericMyAppointmentsFacade()) {
        createUpcomingAppointments {
            respondWithCorrupted(facade)
        }
    }

    fun createTimeoutUpcomingAppointmentsResponse(facade: MyAppointmentsFacade = genericMyAppointmentsFacade()) {
        createUpcomingAppointments {
            respondWithSuccess(facade).delayedBy(Duration.ofSeconds(90))
        }
    }

    fun createUpcomingAppointments(mapping: (IMyAppointmentsBuilder.() -> Mapping)) {
        mockUpcomingAppointments(mapping)
        val facade = genericMyAppointmentsFacade()
        Serenity.setSessionVariable(Slot::class).to(getExpectedUiRepresentationOfSlots(facade))
        setCancellationReasons()
    }

    private fun genericMyAppointmentsFacade(): MyAppointmentsFacade {
        return MyAppointmentsFacade(
                appointmentsFromDate,
                AppointmentsSlotsExample.getGenericExample()
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

    abstract fun getExpectedUiRepresentationOfSlots(facade: MyAppointmentsFacade): List<Slot>

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
}