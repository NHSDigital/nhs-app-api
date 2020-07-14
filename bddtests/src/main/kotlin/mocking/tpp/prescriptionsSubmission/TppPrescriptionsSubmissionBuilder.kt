package mocking.tpp.prescriptionsSubmission

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.RequestMedicationReply
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppPrescriptionsSubmissionBuilder(patient: Patient, drugIds: List<String>?, notes: RequestMedicationReply?=null)
    : TppMappingBuilder("POST", "/tpp/") {

    private var suid: String = ""

    init {
        var matchXPath = "//RequestMedication[" + "@patientId='${patient.patientId}'"
        requestBuilder.andHeader(HEADER_TYPE, "RequestMedication")

        if (drugIds != null && !drugIds.isEmpty()) {
            for (drugId in drugIds) {
                matchXPath = "//Medication[@drugId='$drugId' and @type='Repeat']"
            }
        }
        if (notes != null && drugIds == null){
            matchXPath += "and @notes='${notes.message}']"
        }
        requestBuilder.andBodyMatchingXpath(matchXPath)
    }

    fun respondWithSuccess(requestMedicationReply: RequestMedicationReply): Mapping {

        val jaxbContext = JAXBContext.newInstance(RequestMedicationReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(requestMedicationReply, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(HEADER_SUID, suid)
                    .build()
        }
    }

    fun respondWithError(errorBody: Error): Mapping {
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
        )

        val jaxbContext = JAXBContext.newInstance(Error::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }
}
