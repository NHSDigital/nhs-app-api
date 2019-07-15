package features.im1Appointments.factories

import com.github.tomakehurst.wiremock.stubbing.Scenario
import features.sharedSteps.SupplierSpecificFactory
import mocking.data.appointments.AppointmentSlotsTelephoneExample
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import models.Slot
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import worker.models.appointments.MyAppointmentsResponse
import java.time.Duration
import java.util.*

private const val DELAY_IN_SECONDS = 90L

abstract class MyAppointmentsFactory(gpSupplier: String) : AppointmentsFactory(gpSupplier) {

    fun createSuccessfulEmptyMyAppointmentResponse(
            cancellationReasons: List<AppointmentCancellationReason> = getDefaultCancellationReasons()
    ) {
        val myAppointmentsFacade = MyAppointmentsFacade(
                AppointmentSlotsResponseFacade(
                        cancellationReasons = cancellationReasons,
                        bookingReasonNecessityOption =
                        SerenityHelpers.getValueOrNull<NecessityOption>("BookingReasonNecessity")
                                ?: NecessityOption.NOT_ALLOWED
                )
        )
        mockMyAppointments {
            respondWithSuccess(myAppointmentsFacade)
                    .inScenario("Appointments")
                    .whenScenarioStateIs(Scenario.STARTED)
        }
        setSerenityVariablesForUITests(myAppointmentsFacade)
        generateDefaultUserData()
    }

    fun createSuccessfulMyAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade
            = AppointmentsSlotsExample().getGenericExample(),
            numberOfCancellationReasons: Int = getDefaultCancellationReasons().size
    ) {
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons().subList(
                0,
                numberOfCancellationReasons
        )

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createMyAppointments{
            respondWithSuccess(myAppointmentsFacade)
        }
        Serenity.setSessionVariable(Expectations.EXPECTED_API_RESPONSE_OF_MY_APPOINTMENTS)
                .to(getExpectedApiResponse(myAppointmentsFacade))
        setSerenityVariablesForUITests(myAppointmentsFacade)
    }

    fun createSuccessfulMyTelephoneAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade
            = AppointmentSlotsTelephoneExample().getGenericTelephoneExample(),
            numberOfCancellationReasons: Int = getDefaultCancellationReasons().size
    ) {
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons().subList(
                0,
                numberOfCancellationReasons
        )

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createMyAppointments {
            respondWithSuccess(myAppointmentsFacade)
        }
        Serenity.setSessionVariable(Expectations.EXPECTED_API_RESPONSE_OF_MY_APPOINTMENTS)
                .to(getExpectedApiResponse(myAppointmentsFacade))
        setSerenityVariablesForUITests(myAppointmentsFacade)
    }

    fun createSuccessfulMyAppointmentsResponseOnceBooked(
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
        appointmentSlotsResponseFacade.locations = arrayListOf(
                LocationFacade(
                        sessionWithOnlySelectedSlot.locationId!!,
                        appointmentSlotsFactoryHelper.getLocationNameFromId(sessionWithOnlySelectedSlot)
                )
        )
        appointmentSlotsResponseFacade.staffDetails = appointmentSlotsFactoryHelper
                .extractStaffDetailsFacadesByIds(sessionWithOnlySelectedSlot.staffDetails)
        appointmentSlotsResponseFacade.slotTypes = arrayListOf(
                SlotTypeFacade(
                        sessionWithOnlySelectedSlot.slots.first().slotTypeId,
                        appointmentSlotsFactoryHelper.getSlotTypeNameFromId(sessionWithOnlySelectedSlot.slots.first())
                )
        )
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons().subList(
                0,
                numberOfCancellationReasons
        )

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createMyAppointments {
            respondWithSuccess(myAppointmentsFacade)
                    .inScenario("Appointments")
                    .whenScenarioStateIs("Appointment Booked")
        }
        setSerenityVariablesForUITests(myAppointmentsFacade)
    }

    fun createTimeoutMyAppointmentsResponse(
            appointmentSlotsResponseFacade: AppointmentSlotsResponseFacade = AppointmentsSlotsExample()
                    .getGenericExample()
    ) {
        appointmentSlotsResponseFacade.cancellationReasons = getDefaultCancellationReasons()

        val myAppointmentsFacade = convertToMyAppointmentsFacade(appointmentSlotsResponseFacade)
        createMyAppointments {
            respondWithSuccess(myAppointmentsFacade).delayedBy(Duration.ofSeconds(DELAY_IN_SECONDS))
        }
    }

    fun createMyAppointments(mapping: (IMyAppointmentsBuilder.() -> Mapping)) {
        mockMyAppointments(mapping = mapping)
        generateDefaultUserData()
    }

    private fun convertToMyAppointmentsFacade(facade: AppointmentSlotsResponseFacade): MyAppointmentsFacade {
        return MyAppointmentsFacade(facade)
    }

    open fun mockMyAppointments(
            appointmentType:
            IMyAppointmentsBuilder.AppointmentType = IMyAppointmentsBuilder.AppointmentType.BOTH,
            mapping: (IMyAppointmentsBuilder.() -> Mapping)

    ) {
        appointmentMapper.requestMapping { mapping(viewMyAppointmentsRequest(patient)) }
    }

    private val defaultReasons = arrayListOf(
            AppointmentCancellationReason("R1_NoLongerRequired", "No longer required"),
            AppointmentCancellationReason("R2_UnableToAttend", "Unable to attend")
    )

    open fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = defaultReasons
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    private fun setSerenityVariablesForUITests(myAppointmentsFacade: MyAppointmentsFacade) {
        Serenity.setSessionVariable(MyAppointmentsFacade::class).to(myAppointmentsFacade)
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfUpcomingSlots(myAppointmentsFacade))
        Serenity.setSessionVariable(Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS)
                .to(getExpectedUiRepresentationOfHistoricalSlots(myAppointmentsFacade))
    }

    abstract fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse

    abstract fun getExpectedUiRepresentationOfUpcomingSlots(facade: MyAppointmentsFacade): List<Slot>

    abstract fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot>

    companion object : SupplierSpecificFactory<MyAppointmentsFactory>() {

        override val map: HashMap<String, (() -> (MyAppointmentsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { MyAppointmentsFactoryEmis() },
                    "TPP" to { MyAppointmentsFactoryTpp() },
                    "VISION" to { MyAppointmentsFactoryVision() },
                    "MICROTEST" to { MyAppointmentsFactoryMicrotest() }
            )
        }
    }

    enum class Expectations {
        EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS,
        EXPECTED_UI_REPRESENTATION_OF_MY_HISTORICAL_APPOINTMENTS,
        EXPECTED_API_RESPONSE_OF_MY_APPOINTMENTS
    }
}
