package mocking.defaults.dataPopulation.journies.im1Connection

import mocking.MockingClient
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import models.Patient

class AlreadyRegisteredJourney(private val client: MockingClient) {
    val patient = Patient.halleDawe

    fun create() {
        client
            .forEmis {
                authentication.endUserSessionRequest()
                        .respondWithSuccess(patient.endUserSessionId)
            }

        client
            .forEmis {
                authentication.meApplicationsRequest(
                        patient,
                        LinkApplicationRequestModel(
                                surname = patient.surname,
                                dateOfBirth = patient.dateOfBirth,
                                linkageDetails = LinkageDetailsModel(
                                        accountId = patient.accountId,
                                        linkageKey = patient.linkageKey,
                                        nationalPracticeCode = patient.odsCode
                                )
                        ))
                        .respondWithAlreadyLinked()
            }

        client
            .forEmis {
                authentication.sessionRequest(patient)
                .respondWithSuccess(patient,associationType = AssociationType.Self
                )
            }

        client
            .forEmis {
                myRecord.demographicsRequest(patient)
                .respondWithSuccess(patient,
                        patientIdentifiers = arrayOf(
                                PatientIdentifier(
                                        identifierType = IdentifierType.NhsNumber,
                                        identifierValue = patient.nhsNumbers[0]
                                )
                        )
                )
            }
    }
}