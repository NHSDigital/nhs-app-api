package mocking.tpp.appointments

import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.data.TppConfig
import models.Patient

class MyAppointmentsBuilderTpp(val patient: Patient) : TppMappingBuilder(), IMyAppointmentsBuilder {

    init {
        requestBuilder.andHeader(HEADER_TYPE, "ViewAppointments")
                .andBodyMatchingXpath(
                        xpath = "//ViewAppointments[" +
                                "@apiVersion='${TppConfig.apiVersion}' and " +
                                "@unitId='${TppConfig.unitId}' and " +
                                "@patientId='${patient.patientId}' and " +
                                "@onlineUserId='${patient.onlineUserId}']")
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWithBody(body)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorWhenGPDisabledAppointmentsService()
    }
}