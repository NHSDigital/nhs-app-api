package features.appointments.factories

import constants.DateTimeFormats
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
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.TimeZone
import java.util.Date

abstract class AppointmentsSlotsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(startDate: ZonedDateTime? = null,
                                                       endDate: ZonedDateTime? = null,
                                                       guidanceMessage: Boolean = true) {
        generateExample(generateDefaultUserDataAndRetrieveSlotsExample(), startDate, endDate, guidanceMessage)
    }

    fun generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage() {
        val example = generateDefaultUserDataAndRetrieveSlotsExample()
        generateAppointmentSlotResponseWithoutGuidance(null, null) {
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
            startDate: ZonedDateTime? = null,
            endDate: ZonedDateTime? = null,
            guidanceMessage: Boolean = true) {

        saveToSerenityVariableForRequest(startDate, AppointmentStartTimeKey)
        saveToSerenityVariableForRequest(endDate, AppointmentEndTimeKey)

        val startDateToUseForMockResponse = adjustAndFormatDate(startDate)
        val endDateToUseForMockResponse = adjustAndFormatDate(endDate)

        generateDefaultUserData()

        generateAppointmentSlotResponse(
                startDateToUseForMockResponse,
                endDateToUseForMockResponse,
                guidanceMessage) {
            respondWithSuccess(example)
        }
    }

    private fun adjustAndFormatDate(date: ZonedDateTime?): String? {
        if (date != null) {
            val adjustedDate = date.withZoneSameInstant(supplierAdjustTime)
            return formatDate(adjustedDate)
        }
        return null
    }

    abstract val supplierAdjustTime: ZoneId

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

    private fun saveToSerenityVariableForRequest(date: ZonedDateTime?, key: String) {
        if (date != null) {
            val value = formatDate(date)
            Serenity.setSessionVariable(key).to(value)
        }
    }

    private fun formatDate(date: ZonedDateTime): String {
        val dateFormatter = DateTimeFormatter.ofPattern(
                DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return dateFormatter.format(date)
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
