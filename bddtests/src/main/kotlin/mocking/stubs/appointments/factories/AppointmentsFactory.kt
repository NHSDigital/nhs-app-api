package mocking.stubs.appointments.factories

import constants.DateTimeFormats
import constants.Supplier
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory.Companion.telephoneNumberToEnter
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import models.Patient
import models.Slot
import utils.SerenityHelpers
import java.text.SimpleDateFormat
import java.util.*

abstract class AppointmentsFactory(gpSupplier: Supplier) {

    private val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    val mockingClient = MockingClient.instance
    protected val supplier: Supplier = gpSupplier
    protected val appointmentMapper: MockingClientAppointmentMappingFactory
    protected val appointmentSlotsFactoryHelper = AppointmentSlotsFactoryHelper()
    val patient: Patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)

    init {
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData(defaultPracticeSettings:Boolean = true) {
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient, defaultPracticeSettings)
        CitizenIdSessionCreateJourney().createFor(patient)
        generateSpecificUserData()
    }

    open fun generateSpecificUserData(){}

    fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulEmptyMyAppointmentResponse()
    }

    open fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)

        return Slot(
                date = date,
                time = time,
                sessionName = session.sessionType!!,
                slotType = appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                location = appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                clinicians = setOf(appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session).first()),
                id = slot.slotId,
                channel = slot.channel.toString(),
                telephoneNumber = when (slot.telephoneNumber) {
                    telephoneNumberToEnter -> ""
                    else -> slot.telephoneNumber
                }
        )
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

    private fun createBackendDateTimeFormatWithoutTimezone(): SimpleDateFormat {
        val sdf = SimpleDateFormat(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        sdf.timeZone = timeZone
        return sdf
    }
}
