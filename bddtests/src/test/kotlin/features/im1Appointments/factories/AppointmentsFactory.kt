package features.im1Appointments.factories

import constants.DateTimeFormats
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import models.Patient
import models.Slot
import utils.SerenityHelpers
import java.text.SimpleDateFormat
import java.util.*

abstract class AppointmentsFactory(gpSupplier: String) {

    private val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    val mockingClient = MockingClient.instance
    val patient: Patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)
    protected val supplier: String = gpSupplier
    protected val appointmentMapper: MockingClientAppointmentMappingFactory
    protected val appointmentSlotsFactoryHelper = AppointmentSlotsFactoryHelper()

    init {
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
    }

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
                id = slot.slotId
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
