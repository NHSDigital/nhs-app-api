package mocking.onlineConsultations

import mocking.models.Mapping
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import mocking.onlineConsultations.configurations.isValid.ServiceDefinitionIsValid
import models.Patient
import org.apache.http.HttpStatus
import utils.SerenityHelpers

class IsValidServiceDefinitionBuilder(private var isValid: Boolean) : OnlineConsultationsMappingBuilder("POST",
        "/fhir/ServiceDefinition/\$isValid") {

    private var _configuration: IQuestionConfiguration = ServiceDefinitionIsValid()
    private var patient: Patient = SerenityHelpers.getPatient()

    init {
        requestBuilder
            .andBodyMatchingJsonPath(
                "$.parameter[?(@.name == 'ODSCode' && @.valueString == '${patient.odsCode}')]")
            .andBodyMatchingJsonPath(
                "$.parameter[?(@.name == 'requestId' && @.valueString =~ /[A-Za-z0-9\\-\\.]{1,64}/)]")
    }

    fun respondWithSuccess(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(_configuration.response.replace("{{isValid}}", "$isValid"))
                    .build()
        }
    }
}