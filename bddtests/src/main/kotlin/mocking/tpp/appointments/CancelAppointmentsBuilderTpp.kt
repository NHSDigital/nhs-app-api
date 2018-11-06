package mocking.tpp.appointments

import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.CancelAppointmentReply
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus


class CancelAppointmentsBuilderTpp  (patient: Patient, request: CancelAppointmentSlotFacade)
    : TppMappingBuilder(method = "POST", relativePath = "/tpp/")
        , ICancelAppointmentsBuilder {

    var tppPatient: Patient = patient

    init {
        requestBuilder.andHeader(HEADER_TYPE, "CancelAppointment")
        requestBuilder.andBodyMatchingXpath("//CancelAppointment[" +
                "@patientId='${patient.patientId}' and " +
                "@apptId='${request.slotId}']")
    }

    override fun respondWithSuccess(): Mapping {
        return respondWith(
                CancelAppointmentReply(tppPatient.patientId,
                        uuid = uuid
                ))
    }

    override fun respondWithCorrupted(): Mapping {
        val mapping = respondWithSuccess()

        return respondWith(HttpStatus.SC_OK) {
            andBody(mapping.response!!.body!!.replace(">","|").replace("}","|"), contentType = "application/json")
        }
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("Service unavailable").build()
        }
    }
}