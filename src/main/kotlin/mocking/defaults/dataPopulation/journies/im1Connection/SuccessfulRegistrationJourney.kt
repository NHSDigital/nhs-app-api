package mocking.defaults.dataPopulation.journies.im1Connection

import mocking.MockingClient
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.emis.models.PatientIdentifier
import models.Patient

class SuccessfulRegistrationJourney(private val client: MockingClient) {

    fun create() {
        client.forEmis{endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)}

        client.forEmis {
                    sessionRequest(patient)
                            .respondWithSuccess(patient, associationType = AssociationType.Self)
                }

        client.forEmis {
                    demographicsRequest(patient)
                            .respondWithSuccess(patient,
                                    patientIdentifiers = arrayOf(
                                            PatientIdentifier(
                                                    identifierType = IdentifierType.NhsNumber,
                                                    identifierValue = patient.nhsNumbers[0]
                                            )
                                    )
                            )
                }

        client
                .forEmis {
                    meApplicationsRequest(patient,
                            LinkApplicationRequestModel(
                                    surname = patient.surname,
                                    dateOfBirth = patient.dateOfBirth,
                                    linkageDetails = LinkageDetailsModel(
                                            accountId = patient.accountId,
                                            linkageKey = patient.linkageKey,
                                            nationalPracticeCode = patient.odsCode
                                    )
                            )
                    ).respondWithSuccess(patient.connectionToken)
                }

    }

    companion object {
        val patient = Patient.montelFrye
    }
}