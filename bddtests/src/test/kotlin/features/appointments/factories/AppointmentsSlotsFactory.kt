package features.appointments.factories

import constants.DateTimeFormats
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
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
        generateExample(retrieveSlotsExample(), startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage() {
        val example = retrieveSlotsExample()
        generateAppointmentSlotResponseWithoutGuidance(null, null) {
            respondWithSuccess(example)
        }
        generateDefaultUserData()
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

        generateAppointmentSlotResponse(
                startDateToUseForMockResponse,
                endDateToUseForMockResponse,
                guidanceMessage,
                reasonNecessity
        ) {
            respondWithSuccess(example)
        }

        generateDefaultUserData()
    }

    private fun adjustAndFormatDate(date: ZonedDateTime?): String? {
        if (date != null) {
            val adjustedDate = date.withZoneSameInstant(supplierAdjustTime)
            return formatDate(adjustedDate)
        }
        return null
    }

    private val supplierAdjustTime = TimeZone.getTimeZone("Europe/London").toZoneId()

    private fun retrieveSlotsExample(): AppointmentSlotsResponseFacade {
        val example = AppointmentsSlotsExample.getGenericExample()
        storeUIDateAndTimeOfSlotToSelect()
        return example
    }

    fun generateExample(mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        generateAppointmentSlotResponse(null, null, true, NecessityOption.OPTIONAL, mapping)
        generateDefaultUserData()
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

    open fun generateCorruptedSlotResponse() {
        generateDefaultUserData()

        appointmentMapper.requestMapping {
            appointmentSlotsRequest(patient)
                    .respondWithCorrupted()
        }
    }

    open fun generateServiceUnavailableSlotResponse() {
        generateDefaultUserData()
        appointmentMapper.requestMapping {
            appointmentSlotsRequest(patient)
                    .respondWithUnavailableException()
        }
    }


    companion object : SupplierSpecificFactory<AppointmentsSlotsFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsSlotsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsSlotsFactoryEmis() },
                    "TPP" to { AppointmentsSlotsFactoryTpp() },
                    "VISION" to { AppointmentsSlotsFactoryVision() })
        }
    }
}
