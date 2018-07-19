package mocking.tpp

import mocking.IAppointmentMappingBuilder
import mocking.IBookAppointmentsBuilder
import mocking.MappingBuilder
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


open class TppMappingBuilder(private val method: String, relativePath: String) : MappingBuilder(method, "$relativePath"), IAppointmentMappingBuilder {

    internal val HEADER_TYPE = "type"
    internal val HEADER_SUID = "suid"
    var delayMillisecs = 0

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)
    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)
    fun listRepeatMedication(patient: Patient) = TppPrescriptionsBuilder(patient)
    fun patientRecordRequest(tppUserSession: TppUserSession) = TppRequestPatientRecordBuilder(tppUserSession)
    fun testResultsViewRequest(tppUserSession: TppUserSession, startDate: OffsetDateTime, endDate: OffsetDateTime) = TppTestResultsViewBuilder(tppUserSession, startDate, endDate)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder =
        TppBookAppointmentsBuilder(patient, request)

    protected inline fun <reified T : Any> respondWith(thing : T):Mapping{
        val jaxbContext = JAXBContext.newInstance(T::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(thing, stringWriter)
        }

        var xmlBody = stringWriter.toString()

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody)
                    .andHeader("type", "")
                    .andDelay(delayMillisecs)
                    .build()
        }
    }

}
