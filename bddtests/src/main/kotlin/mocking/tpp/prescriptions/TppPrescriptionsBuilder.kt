package mocking.tpp.prescriptions

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.ListRepeatMedicationReply
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppPrescriptionsBuilder(patient: Patient)
    : TppMappingBuilder("POST", "/tpp/") {

    private var suid: String = ""

    init {
        requestBuilder.andHeader(HEADER_TYPE, "ListRepeatMedication")
        requestBuilder.andBodyMatchingXpath("//ListRepeatMedication[" +
                "@patientId='${patient.patientId}']")
    }

    fun respondWithSuccess(listRepeatMedicationReply: ListRepeatMedicationReply): Mapping {

        val jaxbContext = JAXBContext.newInstance(ListRepeatMedicationReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(listRepeatMedicationReply, stringWriter)
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
