package mocking.onlineConsultations

import mocking.models.Mapping
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import models.NhsNumberFormatter
import models.Patient
import org.apache.http.HttpStatus
import utils.SerenityHelpers

class EvaluateServiceDefinitionBuilder(hasGpSession: Boolean, serviceDefinitionId: String,
                                       configuration: IQuestionConfiguration)
    : OnlineConsultationsMappingBuilder("POST",
                                        "/fhir/ServiceDefinition/$serviceDefinitionId/\$evaluate") {

        /* The gpSession variable can be removed when we can get the address from NHS login
        and all other related code within the OLC BDDs */
        private var _configuration: IQuestionConfiguration = configuration
        private var patient: Patient = SerenityHelpers.getPatient()
        private val address = if (hasGpSession) {
            patient.contactDetails.address.full()
        } else {
            ""
        }

    init {
        //for the OLC requests the NHS number needs to be formatted as 000 000 0000
        val formattedNhsNumber = NhsNumberFormatter.format(patient.nhsNumbers[0])
        val request = _configuration.request.replace("{{odsCode}}", patient.odsCode)
                .replace("{{dob}}", patient.age.dateOfBirth)
                .replace("{{nhsNumber}}", formattedNhsNumber)
                .replace("{{familyName}}", patient.name.surname)
                .replace("{{name}}", patient.name.firstName)
                .replace("{{address}}", address)
        requestBuilder.andHeader("provider", "eConsult")
        requestBuilder.andHeader("Content-Type", "application/json; charset=UTF-8")
        requestBuilder.andBody(request)
    }

    fun respondWithSuccess(): Mapping {
        //The replace will need removed when we can get the address from NHS login
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(_configuration.response.replace("{{address}}", address))
                    .build()
        }
    }
}
