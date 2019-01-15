package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import java.time.ZonedDateTime
import java.util.*

private const val DEFAULT_NUMBER_OF_DAYS_IN_RANGE = 29L

abstract class AppointmentsSlotsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    private val supplierAdjustTime = TimeZone.getTimeZone("Europe/London").toZoneId()

    fun generateDefaultAvailableAppointmentSlotExample(startDate: ZonedDateTime? = null,
                                                       endDate: ZonedDateTime? = null,
                                                       guidanceMessage: String? = null,
                                                       reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {
        generateExample(retrieveSlotsExample(), startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateAvailableSlotExampleIncludingTelephoneAppointment(startDate: ZonedDateTime? = null,
                                                       endDate: ZonedDateTime? = null,
                                                       guidanceMessage: String? = null,
                                                       reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {
        generateExample(retrieveSlotsExampleIncludingTelephoneAppointments(),
                            startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage() {
        val example = retrieveSlotsExample()
        generateAppointmentSlotResponseWithoutGuidance(defaultStartDate(), defaultEndDate()) {
            respondWithSuccess(example)
        }
        generateAppointmentSlotResponseWithoutGuidance(defaultStartDate().plusMinutes(1), defaultEndDate()) {
            respondWithSuccess(example)
        }
        generateDefaultUserData()
        createGetEmptyAppointmentList()
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
            guidanceMessage: String? = null,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {

        val startDateToUseForMockResponse = startDate ?: defaultStartDate()
        val endDateToUseForMockResponse = endDate ?: defaultEndDate()
        Serenity.setSessionVariable("BookingReasonNecessity").to(reasonNecessity)

        generateAppointmentSlotResponse(
                startDateToUseForMockResponse,
                endDateToUseForMockResponse,
                guidanceMessage,
                reasonNecessity
        ) {
            respondWithSuccess(example)
        }

        generateAppointmentSlotResponse(
                startDateToUseForMockResponse.plusMinutes(1),
                endDateToUseForMockResponse,
                guidanceMessage,
                reasonNecessity
        ) {
            respondWithSuccess(example)
        }
        generateDefaultUserData()
        createGetEmptyAppointmentList()
    }

    private fun retrieveSlotsExample(): AppointmentSlotsResponseFacade {
        val example = AppointmentsSlotsExample.getGenericExample()
        storeUIDateAndTimeOfSlotToSelect()
        return example
    }

    private fun retrieveSlotsExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {
        val example = AppointmentsSlotsExample.slotExampleIncludingTelephoneAppointments()
        storeUIDateAndTimeOfSlotToSelect()
        return example
    }

    fun generateExample(mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        generateAppointmentSlotResponse(
                defaultStartDate(),
                defaultEndDate(),
                null,
                NecessityOption.OPTIONAL,
                mapping
        )
        generateAppointmentSlotResponse(
                defaultStartDate().plusMinutes(1),
                defaultEndDate(),
                null,
                NecessityOption.OPTIONAL,
                mapping
        )
        generateDefaultUserData()
        createGetEmptyAppointmentList()
    }

    private fun defaultStartDate(): ZonedDateTime {
        return ZonedDateTime.now()
                .withZoneSameInstant(supplierAdjustTime)
    }

    private fun defaultEndDate(): ZonedDateTime {
        return ZonedDateTime.now()
                .toLocalDate()
                .atStartOfDay(supplierAdjustTime)
                .plusDays(DEFAULT_NUMBER_OF_DAYS_IN_RANGE)
    }

    private fun storeUIDateAndTimeOfSlotToSelect() {
        val expectedFilteredSlots = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_FILTER_FACADE_KEY
        ).filteredSlots
        val dateToSelect = expectedFilteredSlots.keys.first()
        val timeToSelect = expectedFilteredSlots[dateToSelect]!!.first()
        setSessionVariable(TargetAppointmentDateKey).to(dateToSelect)
        setSessionVariable(TargetAppointmentTimeKey).to(timeToSelect)
    }

    abstract fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: String?,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping)

    abstract fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
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
