package mocking.tpp.appointments

import mocking.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.CancelAppointmentReply
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient


class TppCancelAppointmentsBuilder  (patient: Patient, request: CancelAppointmentSlotFacade)
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
}