package mocking.tpp.prescriptionsSubmission

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.RequestMedicationReply
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppPrescriptionsSubmissionBuilder(patient: Patient, drugIds: List<String>?)
    : TppMappingBuilder("POST", "/tpp/") {

    private var Suid: String = ""

    init {
        requestBuilder.andHeader(HEADER_TYPE, "RequestMedication")
        requestBuilder.andBodyMatchingXpath("//RequestMedication[" +
                "@patientId='${patient.patientId}']")

        if (drugIds != null && !drugIds.isEmpty()) {
            for (drugId in drugIds) {
                requestBuilder.andBodyMatchingXpath(
                        "//Medication[@drugId='$drugId' and @type='Repeat']")
            }
        }
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
                    .andHeader(HEADER_SUID, Suid)
                    .build()
        }
    }
}