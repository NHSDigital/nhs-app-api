package features.appointments.factories

import constants.DateTimeFormats
import features.appointments.data.AppointmentsSlotsExample
import features.appointments.data.AppointmentsSlotsExampleBuilderWithExpectations
import features.sharedSteps.SupplierSpecificFactory
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*

abstract class AppointmentsSlotsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(startDate: ZonedDateTime? = null,
                                                       endDate: ZonedDateTime? = null,
                                                       guidanceMessage: Boolean = true,
                                                       reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {
        generateExample(generateDefaultUserDataAndRetrieveSlotsExample(), startDate, endDate, guidanceMessage, reasonNecessity)
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
            guidanceMessage: Boolean = true,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {

        saveToSerenityVariableForRequest(startDate, AppointmentStartTimeKey)
        saveToSerenityVariableForRequest(endDate, AppointmentEndTimeKey)

        val startDateToUseForMockResponse = adjustAndFormatDate(startDate)
        val endDateToUseForMockResponse = adjustAndFormatDate(endDate)

        generateDefaultUserData()

        generateAppointmentSlotResponse(
                startDateToUseForMockResponse,
                endDateToUseForMockResponse,
                guidanceMessage,
                reasonNecessity
        ) {
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

    private val supplierAdjustTime = TimeZone.getTimeZone("Europe/London").toZoneId()

    private fun generateDefaultUserDataAndRetrieveSlotsExample(): AppointmentSlotsResponseFacade {
        generateDefaultUserData()
        val example = AppointmentsSlotsExample.getGenericExample()
        storeUIDateAndTimeOfSlotToSelect()
        return example
    }

    fun generateExample(mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        generateDefaultUserData()
        generateAppointmentSlotResponse(null, null, true, NecessityOption.OPTIONAL, mapping)
    }

    private fun saveToSerenityVariableForRequest(date: ZonedDateTime?, key: String) {
        if (date != null) {
            val value = formatDate(date)
            Serenity.setSessionVariable(key).to(value)
        }
    }

    private fun formatDate(date: ZonedDateTime): String {
        val dateFormatter = DateTimeFormatter.ofPattern(
                DateTimeFormats.backendDateTimeFormatWithTimezone)
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
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping)

    abstract fun generateAppointmentSlotResponseWithoutGuidance(startDate: String?,
                                                                endDate: String?,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping))


    companion object : SupplierSpecificFactory<AppointmentsSlotsFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsSlotsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsSlotsFactoryEmis() },
                    "TPP" to { AppointmentsSlotsFactoryTpp() },
                    "VISION" to { AppointmentsSlotsFactoryVision() })
        }
    }
}
