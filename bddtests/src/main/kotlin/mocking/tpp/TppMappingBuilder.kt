package mocking.tpp

import mocking.IAppointmentMappingBuilder
import mocking.IAppointmentSlotsBuilder
import mocking.IBookAppointmentsBuilder
import mocking.MappingBuilder
import mocking.tpp.appointments.TppAppointmentSlotsBuilder
import mocking.models.Mapping
import mocking.tpp.appointments.TppBookAppointmentsBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.prescriptions.TppPrescriptionsBuilder
import mocking.tpp.requestPatientRecord.TppRequestPatientRecordBuilder
import mocking.tpp.session.TppSessionBuilder
import mocking.tpp.testResultsView.TppTestResultsViewBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import java.time.OffsetDateTime
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller


open class TppMappingBuilder(private val method: String, relativePath: String) : MappingBuilder(method, relativePath)
        , IAppointmentMappingBuilder {


    internal val HEADER_TYPE = "type"
    internal val HEADER_SUID = "suid"
    var delayMillisecs = 0

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)
    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)
    fun listRepeatMedication(patient: Patient) = TppPrescriptionsBuilder(patient)
    fun patientRecordRequest(tppUserSession: TppUserSession) = TppRequestPatientRecordBuilder(tppUserSession)
    fun testResultsViewRequest(tppUserSession: TppUserSession, startDate: OffsetDateTime, endDate: OffsetDateTime) = TppTestResultsViewBuilder(tppUserSession, startDate, endDate)
    override fun appointmentSlotsRequest(patient: Patient, fromDateTime: String?, toDateTime: String?) = TppAppointmentSlotsBuilder(patient.tppUserSession!!)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder =
        TppBookAppointmentsBuilder(patient, request)

    protected inline fun <reified T : Any> respondWith(response : T):Mapping{

        var xmlBody = serialsier(response)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody)
                    .andHeader("type", "")
                    .andDelay(delayMillisecs)
                    .build()
        }
    }

    protected inline fun <reified T : Any> serialsier(response : T):String{
        val jaxbContext = JAXBContext.newInstance(T::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(response, stringWriter)
        }

        return stringWriter.toString()
    }

    companion object {

        const val uuid="3e3d8bef-4ce1-4925-a263-149c15ac7208"
    }

}
