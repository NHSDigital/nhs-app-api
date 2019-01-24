package features.appointments.factories

import constants.DateTimeFormats
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import models.Patient
import models.Slot
import net.serenitybdd.core.Serenity
import java.text.SimpleDateFormat
import java.util.*

abstract class AppointmentsFactory(gpSupplier: String) {

    protected val timeZone = TimeZone.getTimeZone("Europe/London")
    protected val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    val mockingClient = MockingClient.instance
    val patient: Patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)
    protected val supplier: String = gpSupplier
    protected val appointmentMapper: MockingClientAppointmentMappingFactory

    init {
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        Serenity.setSessionVariable(GLOBAL_PROVIDER_TYPE).to(supplier)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
    }

    fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse()
    }

    open fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val sessionDetails = "${session.sessionType} - ${slot.slotTypeName}"
        val location = session.location
        return Slot(
                date = date,
                time = time,
                session = sessionDetails,
                location = location!!,
                clinicians = setOf(session.staffDetails.first().staffName!!),
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

    companion object {
        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}
