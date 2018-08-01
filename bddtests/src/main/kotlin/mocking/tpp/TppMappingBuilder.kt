package mocking.tpp

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.MappingBuilder
import mocking.tpp.appointments.AppointmentSlotsBuilderTpp
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.*
import mocking.models.Mapping
import mocking.tpp.appointments.BookAppointmentsBuilderTpp
import mocking.tpp.appointments.MyAppointmentsBuilderTpp
import mocking.tpp.appointments.CancelAppointmentsBuilderTpp
import mocking.tpp.models.Authenticate
import mocking.tpp.models.LinkAccount
import mocking.tpp.models.Error
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.prescriptions.TppPrescriptionsBuilder
import mocking.tpp.prescriptionsSubmission.TppPrescriptionsSubmissionBuilder
import mocking.tpp.registration.LinkAccountBuilder
import mocking.tpp.requestPatientRecord.TppRequestPatientRecordBuilder
import mocking.tpp.session.TppSessionBuilder
import mocking.tpp.testResultsView.TppTestResultsViewBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.time.OffsetDateTime
import java.util.*

open class TppMappingBuilder(method: String = "POST", relativePath: String = "/tpp/") : MappingBuilder(method, relativePath), IAppointmentMappingBuilder {

    private val HEADER_CONTENT_TYPE = "Content-Type"
    internal val HEADER_TYPE = "type"
    internal val HEADER_SUID = "suid"

    var delayMillisecs = 0

    init {
        requestBuilder.andHeader(HEADER_CONTENT_TYPE, "text/xml; charset=UTF-8")
    }

    override fun appointmentSlotsRequest(patient: Patient, fromDateTime: String?, toDateTime: String?) = AppointmentSlotsBuilderTpp(patient.tppUserSession!!)

    override fun viewMyAppointmentsRequest(patient: Patient): IMyAppointmentsBuilder = MyAppointmentsBuilderTpp(patient)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder =
            BookAppointmentsBuilderTpp(patient, request)

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)

    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)

    fun listRepeatMedication(patient: Patient) = TppPrescriptionsBuilder(patient)

    fun linkAccountRequest(patient: Patient) = LinkAccountBuilder(LinkAccount.forPatient(patient))

    fun prescriptionSubmission(patient: Patient, drugIds: List<String>?) = TppPrescriptionsSubmissionBuilder(patient, drugIds)

    fun patientRecordRequest(tppUserSession: TppUserSession) = TppRequestPatientRecordBuilder(tppUserSession)

    fun testResultsViewRequest(tppUserSession: TppUserSession, startDate: OffsetDateTime, endDate: OffsetDateTime) = TppTestResultsViewBuilder(tppUserSession, startDate, endDate)

    fun responseErrorWhenGPDisabledAppointmentsService(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(errorCode = "6", userFriendlyMessage = errorMsg, uuid = UUID.randomUUID().toString())
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(JSonXmlConverter.toXML(disabledTppError))
        }
    }

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade): ICancelAppointmentsBuilder =
            CancelAppointmentsBuilderTpp(patient, request)

    protected inline fun <reified T : Any> respondWith(response: T): Mapping {

        var xmlBody = JSonXmlConverter.toXML(response)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody)
                    .andHeader("type", "")
                    .andDelay(delayMillisecs)
                    .build()
        }
    }

    companion object {
        const val uuid = "3e3d8bef-4ce1-4925-a263-149c15ac7208"
    }

}
