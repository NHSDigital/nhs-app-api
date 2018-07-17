package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import models.Patient
import mocking.emis.demographics.PatientIdentifier

class EmisSessionCreateJourneyFactory(val client: MockingClient): SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        client
                .forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(patient.endUserSessionId)
                }

        client
                .forEmis {
                    sessionRequest(patient)
                            .respondWithSuccess(patient, AssociationType.Self)
                }
        client
                .forEmis {
                    demographicsRequest(patient)
                            .respondWithSuccess(patient,
                                    patientIdentifiers = arrayOf(
                                            PatientIdentifier(
                                                    identifierType = IdentifierType.NhsNumber,
                                                    identifierValue = MockDefaults.patient.nhsNumbers[0]
                                            )
                                    )
                            )
                }
    }
}