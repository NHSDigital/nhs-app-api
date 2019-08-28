package mocking.stubs.appointments.factories

import mocking.SupplierSpecificFactory
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentSlotsTelephoneExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import worker.models.appointments.SlotResponseObject
import java.time.ZonedDateTime
import java.util.*

private const val APPOINTMENT_SLOT_RESPONSE_VALIDITY_TIME = 10L

abstract class AppointmentsSlotsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    private val appointmentSlotsExample = AppointmentsSlotsExample()
    private val appointmentSlotsTelephoneExample = AppointmentSlotsTelephoneExample()

    fun generateDefaultAvailableAppointmentSlotExample(
            startDate: ZonedDateTime? = null,
            endDate: ZonedDateTime? = null,
            guidanceMessage: String? = null,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY
    ) {
        generateExample(retrieveSlotsExample(), startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateAvailableSlotExampleIncludingTelephoneAppointment(
            startDate: ZonedDateTime? = null,
            endDate: ZonedDateTime? = null,
            guidanceMessage: String? = null,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY,
            telephoneNumber: String =""
    ) {
        generateExample(retrieveSlotsExampleIncludingTelephoneAppointments(telephoneNumber),
                startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateAvailableSlotExampleIncludingTelephoneAppointment1(
            startDate: ZonedDateTime? = null,
            endDate: ZonedDateTime? = null,
            guidanceMessage: String? = null,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY
    ) {
        generateExample(retrieveSlotsExampleIncludingTelephoneAppointments1(),
                startDate, endDate, guidanceMessage, reasonNecessity)
    }

    fun generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage() {
        val example = retrieveSlotsExample()
        generateAppointmentSlotResponseWithoutGuidance(appointmentSlotsFactoryHelper.defaultStartDate(),
                appointmentSlotsFactoryHelper.defaultEndDate()) {
            respondWithSuccess(example)
        }
        generateAppointmentSlotResponseWithoutGuidance(appointmentSlotsFactoryHelper.defaultStartDate().plusMinutes
        (1), appointmentSlotsFactoryHelper.defaultEndDate()) {
            respondWithSuccess(example)
        }
        generateDefaultUserData()
        createGetEmptyAppointmentList()
    }

    fun generateMultipleAvailableAppointmentSlotsForTheSameTime() {
        val example = appointmentSlotsExample.multipleSlotsOneTime()
        generateExample(example)
    }

    fun generateExample(
            example: AppointmentSlotsResponseFacade,
            startDate: ZonedDateTime? = null,
            endDate: ZonedDateTime? = null,
            guidanceMessage: String? = null,
            reasonNecessity: NecessityOption = NecessityOption.MANDATORY) {

        val startDateToUseForMockResponse = startDate ?: appointmentSlotsFactoryHelper.defaultStartDate()
        val endDateToUseForMockResponse = endDate ?: appointmentSlotsFactoryHelper.defaultEndDate()
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
                startDateToUseForMockResponse.plusMinutes(APPOINTMENT_SLOT_RESPONSE_VALIDITY_TIME),
                endDateToUseForMockResponse,
                guidanceMessage,
                reasonNecessity
        ) {
            respondWithSuccess(example)
        }

        val expected = getExpectedApiResponseSlots(example)
        println(expected)
        val expectedUi = getExpectedUiRepresentationOfFilteredSlots(
                sessionVariableCalled<AppointmentFilterFacade>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .APPOINTMENT_FILTER_FACADE_KEY
                )
        )
        println(expectedUi)
        Serenity.setSessionVariable(Expectations.EXPECTED_API_RESPONSE_OF_AVAILABLE_APPOINTMENTS)
                .to(getExpectedApiResponseSlots(example))

        val expectedUiFiltered =  getExpectedUiRepresentationOfFilteredSlots(
                sessionVariableCalled<AppointmentFilterFacade>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .APPOINTMENT_FILTER_FACADE_KEY
                )
        )
        println(expectedUiFiltered)
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS)
                .to(
                        getExpectedUiRepresentationOfFilteredSlots(
                                sessionVariableCalled<AppointmentFilterFacade>(
                                        AppointmentsSlotsExampleBuilderWithExpectations
                                                .AppointmentSlotSerenityKeys
                                                .APPOINTMENT_FILTER_FACADE_KEY
                                )
                        )
                )
        appointmentSlotsFactoryHelper.storeUIDetailsOfSlotToSelect()
        generateDefaultUserData()
        createGetEmptyAppointmentList()
    }

    private fun retrieveSlotsExample() = appointmentSlotsExample.getGenericExample()

    private fun retrieveSlotsExampleIncludingTelephoneAppointments(telephoneNumber: String) =
            appointmentSlotsTelephoneExample.slotExampleIncludingTelephoneAppointments(telephoneNumber)

    private fun retrieveSlotsExampleIncludingTelephoneAppointments1() =
        appointmentSlotsTelephoneExample.slotExampleIncludingTelephoneAppointments("")

    fun generateExample(mapping: (IAppointmentSlotsBuilder.() -> Mapping)) {
        generateAppointmentSlotResponse(
                appointmentSlotsFactoryHelper.defaultStartDate(),
                appointmentSlotsFactoryHelper.defaultEndDate(),
                null,
                NecessityOption.OPTIONAL,
                mapping
        )

        generateAppointmentSlotResponse(
                appointmentSlotsFactoryHelper.defaultStartDate().plusMinutes(APPOINTMENT_SLOT_RESPONSE_VALIDITY_TIME),
                appointmentSlotsFactoryHelper.defaultEndDate(),
                null,
                NecessityOption.OPTIONAL,
                mapping
        )
        generateDefaultUserData()
        createGetEmptyAppointmentList()
    }

    abstract fun generateAppointmentSlotResponse(startDate: ZonedDateTime,
                                                 endDate: ZonedDateTime,
                                                 guidanceMessage: String?,
                                                 reasonNecessity: NecessityOption,
                                                 mapping: IAppointmentSlotsBuilder.() -> Mapping)

    abstract fun generateAppointmentSlotResponseWithoutGuidance(startDate: ZonedDateTime,
                                                                endDate: ZonedDateTime,
                                                                mapping: (IAppointmentSlotsBuilder.() -> Mapping))

    abstract fun getExpectedApiResponseSlots(facade: AppointmentSlotsResponseFacade):
            List<SlotResponseObject>

    abstract fun getExpectedUiRepresentationOfFilteredSlots(facade: AppointmentFilterFacade):
            AppointmentFilterFacade

    companion object : SupplierSpecificFactory<AppointmentsSlotsFactory>() {

        override val map: HashMap<String, (() -> (AppointmentsSlotsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsSlotsFactoryEmis() },
                    "TPP" to { AppointmentsSlotsFactoryTpp() },
                    "VISION" to { AppointmentsSlotsFactoryVision() },
                    "MICROTEST" to { AppointmentsSlotsFactoryMicrotest() }
            )
        }
    }

    enum class Expectations {
        EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS,
        EXPECTED_API_RESPONSE_OF_AVAILABLE_APPOINTMENTS
    }
}
