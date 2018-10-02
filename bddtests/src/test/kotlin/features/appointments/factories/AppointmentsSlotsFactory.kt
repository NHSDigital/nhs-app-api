package features.appointments.factories

import features.appointments.data.AppointmentsBookingData
import features.appointments.data.AppointmentsSlotsExample
import features.appointments.data.AppointmentsSlotsExampleBuilderWithExpectations
import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.ZoneOffset
import java.util.Date
import java.util.Locale
import java.util.TimeZone

abstract class AppointmentsSlotsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(startDate: LocalDateTime? = null,
                                                       endDate: LocalDateTime? = null,
                                                       guidanceMessage: Boolean = true) {
        generateExample(generateDefaultUserDataAndRetrieveSlotsExample(), startDate, endDate, guidanceMessage)
    }

    fun generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage(
            startDate: LocalDateTime? = null,
            endDate: LocalDateTime? = null
    ) {
        val example = generateDefaultUserDataAndRetrieveSlotsExample()
        val startDateToUse = getFormattedDate(startDate, AppointmentStartTimeKey)
        val endDateToUse = getFormattedDate(endDate, AppointmentEndTimeKey)

        generateAppointmentSlotResponseWithoutGuidance(startDateToUse, endDateToUse) {
            respondWithSuccess(example)
        }
    }

    fun generateMultipleAvailableAppointmentSlotsForTheSameTime() {
        val example = AppointmentsSlotsExample.multipleSlotsOneTime()
        storeUIDateAndTimeOfSlotToSelect()
        generateExample(example)
    }

    fun generateExample(
            example: AppointmentSlotsResponseFacade,
            startDate: LocalDateTime? = null,
            endDate: LocalDateTime? = null,
            guidanceMessage: Boolean = true) {
        val startDateToUse = getFormattedDate(startDate, AppointmentStartTimeKey)
        val endDateToUse = getFormattedDate(endDate, AppointmentEndTimeKey)

        generateDefaultUserData()

        generateAppointmentSlotResponse(startDateToUse, endDateToUse, guidanceMessage) {
            respondWithSuccess(example)
        }
    }

    private fun generateDefaultUserDataAndRetrieveSlotsExample(): AppointmentSlotsResponseFacade {
        generateDefaultUserData()
        val example = AppointmentsSlotsExample.getGenericExample()
        storeUIDateAndTimeOfSlotToSelect()
        return example
    }

    fun generateExample(mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        generateDefaultUserData()
        generateAppointmentSlotResponse(null, null, true, mapping)
    }

    private fun getFormattedDate(date: LocalDateTime?, key: String): String? {
        if (date != null) {
            Serenity.setSessionVariable(key).to(getRequestDateTime(date))
            return date.format(AppointmentsBookingData.dateTimeFormat)
        }
        return null
    }

    abstract val zoneOffset: ZoneOffset

    private fun getRequestDateTime(date: LocalDateTime): String {
        val sdf = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.UK)
        sdf.timeZone = TimeZone.getTimeZone("UTC")
        return sdf.format(Date.from(date.toInstant(zoneOffset)))
    }

    private fun storeUIDateAndTimeOfSlotToSelect() {
        val expectedFilteredSlots = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotExpectations
                        .EXPECTED_APPOINTMENT_FILTER_FACADE_KEY
        ).filteredSlots
        val dateToSelect = expectedFilteredSlots.keys.first()
        val timeToSelect = expectedFilteredSlots[dateToSelect]!!.first()
        setSessionVariable(TargetAppointmentDateKey).to(dateToSelect)
        setSessionVariable(TargetAppointmentTimeKey).to(timeToSelect)
    }

    abstract fun generateAppointmentSlotResponse(startDate: String?,
                                                 endDate: String?,
                                                 guidanceMessage: Boolean,
                                                 mapping: (IAppointmentSlotsBuilder.() -> Mapping))

    abstract fun generateAppointmentSlotResponseWithoutGuidance(startDate: String?,
                                                                endDate: String?,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping))


    companion object : SupplierSpecificFactory<AppointmentsSlotsFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsSlotsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsSlotsFactoryEmis() },
                    "TPP" to { AppointmentsSlotsFactoryTpp() })
        }
    }
}
