package mocking.onlineConsultations

import mocking.emis.OnlineConsultationsMappingBuilder
import mocking.models.Mapping
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import models.NhsNumberFormatter
import models.Patient
import org.apache.http.HttpStatus
import utils.SerenityHelpers

class EvaluateServiceDefinitionBuilder(serviceDefinitionId: String,
                                       configuration: IQuestionConfiguration)
    : OnlineConsultationsMappingBuilder("POST",
                                        "/fhir/ServiceDefinition/$serviceDefinitionId/\$evaluate") {

        private var _configuration: IQuestionConfiguration = configuration
        private var patient: Patient = SerenityHelpers.getPatient()

    init {
            //for the OLC requests the NHS number needs to be formatted as 000 000 0000
            val formattedNhsNumber = NhsNumberFormatter.format(patient.nhsNumbers[0])
            val request = _configuration.request.replace("{{odsCode}}", patient.odsCode)
                    .replace("{{address}}", patient.address.full())
                    .replace("{{dob}}", patient.dateOfBirth)
                    .replace("{{nhsNumber}}", formattedNhsNumber)
                    .replace("{{familyName}}", patient.surname)
                    .replace("{{name}}", patient.firstName)
            requestBuilder.andHeader("provider", "eConsult")
            requestBuilder.andHeader("Content-Type", "application/json; charset=UTF-8")
            requestBuilder.andBody(request)
        }

        fun respondWithSuccess(): Mapping {
            return respondWith(HttpStatus.SC_OK) {
                andJsonBody(_configuration.response)
                        .build()
            }
        }
}
