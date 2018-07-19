package mocking.defaults.dataPopulation.journies.im1Connection

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.*
import models.Patient

class SuccessfulRegistrationJourney(private val client: MockingClient) {

    fun create(patient: Patient = SuccessfulRegistrationJourney.patient, gpSystem: String = "EMIS") {
        when (gpSystem) {
            "EMIS" -> generateEmisMocks(patient)
            "TPP" -> generateTppMocks(patient)
            else -> throw IllegalArgumentException("$gpSystem not recognised as a supported GP System.")
        }
    }

    private fun generateEmisMocks(patient: Patient) {
        client.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }

        client.forEmis {
            sessionRequest(patient)
                    .respondWithSuccess(patient, associationType = AssociationType.Self)
        }

        client.forEmis {
            demographicsRequest(patient)
                    .respondWithSuccess(patient,
                            patientIdentifiers =
                            patient.nhsNumbers.map {
                                PatientIdentifier(
                                        identifierType = IdentifierType.NhsNumber,
                                        identifierValue = it
                                )
                            }.toTypedArray()
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

    private fun generateTppMocks(patient: Patient) {

        client.forTpp {
                linkAccountRequest(patient).respondWithSuccess(
                                LinkAccountReply(
                                        passphrase = patient.passphrase,
                                        uuid = MockDefaults.DEFAULT_TPP_UUID
                                )
                        )
        }

        client.forTpp {
            authenticateRequest(
                    Authenticate(
                            apiVersion = MockDefaults.TPP_API_VERSION,
                            accountId = patient.accountId,
                            passphrase = patient.passphrase,
                            unitId = patient.odsCode,
                            uuid = MockDefaults.DEFAULT_TPP_UUID,
                            application = Application(
                                    name = "NhsApp",
                                    version = "1.0",
                                    providerId = MockDefaults.DEFAULT_TPP_PROVIDER_ID,
                                    deviceType = "NhsApp"
                            ))
                    )
                    .respondWithSuccess(
                            AuthenticateReply(
                                    patientId = patient.patientId,
                                    onlineUserId = patient.onlineUserId,
                                    uuid = "af0a8175-e6c2-4c49-883e-020b2b3600f9",
                                    user = User(
                                            person = Person(
                                                    patientId = patient.patientId,
                                                    dateOfBirth = patient.dateOfBirth,
                                                    gender = patient.sex.name,
                                                    nationalId = NationalId(
                                                            type = "NHS",
                                                            value = patient.nhsNumbers.first()
                                                    ),
                                                    personName = PersonName(
                                                            name = "${patient.title} ${patient.firstName} ${patient.surname}"
                                                    )
                                            )
                                    )
                            )
                    )
        }
    }

    companion object {
        val patient = Patient.montelFrye
    }
}