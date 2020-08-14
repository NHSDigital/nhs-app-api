package mocking.defaults.dataPopulation.journies.im1Connection

import constants.Supplier
import constants.TppConstants
import mocking.MockingClient
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.LinkAccountReply
import mocking.tpp.models.NationalId
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.User
import mocking.defaults.VisionMockDefaults
import mocking.emis.practices.SettingsResponseModel
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.Register
import mocking.vision.models.VisionUserSession
import models.Patient

class SuccessfulRegistrationJourney(private val client: MockingClient) {

    fun create(patient: Patient, gpSystem: Supplier) {
        when (gpSystem) {
            Supplier.EMIS -> generateEmisMocks(patient)
            Supplier.TPP -> generateTppMocks(patient)
            Supplier.VISION -> generateVisionMocks(patient)
            Supplier.MICROTEST -> { /* Microtest does not support registration with linkage keys */ }
        }
    }

    private fun generateEmisMocks(patient: Patient) {
        client.forEmis.mock { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }

        client.forEmis.mock {
            authentication.sessionRequest(patient)
                    .respondWithSuccess(patient, associationType = AssociationType.Self)
        }

        client.forEmis.mock {
            myRecord.demographicsRequest(patient)
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
                .forEmis.mock {
                    authentication.meApplicationsRequest(patient,
                            LinkApplicationRequestModel(
                                    surname = patient.name.surname,
                                    dateOfBirth = patient.age.dateOfBirth.plus("T00:00:00"),
                                    linkageDetails = LinkageDetailsModel(
                                            accountId = patient.accountId,
                                            linkageKey = patient.linkageKey,
                                            nationalPracticeCode = patient.odsCode
                                    )
                            )
                    ).respondWithSuccess(patient.connectionToken)
                }
        client.forEmis.mock {
            practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel())
        }
    }

    private fun generateTppMocks(patient: Patient) {

        client.forTpp.mock {
                authentication.linkAccountRequest(patient).respondWithSuccess(
                                LinkAccountReply(
                                        passphrase = patient.passphrase,
                                        uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                                        passphraseToLink = "passphraseToLink"
                                )
                        )
        }

        client.forTpp.mock {
            authentication.authenticateRequest(
                    Authenticate(
                            apiVersion = TppMockDefaults.TPP_API_VERSION,
                            accountId = patient.accountId,
                            passphrase = patient.passphrase,
                            unitId = patient.odsCode,
                            uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                            application = Application(
                                    name = "NhsApp",
                                    version = "1.0",
                                    providerId = TppMockDefaults.DEFAULT_TPP_PROVIDER_ID,
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
                                                    dateOfBirth = patient.age.dateOfBirth,
                                                    gender = patient.sex.name,
                                                    nationalId = NationalId(
                                                            type = TppConstants.NationalIdTypeNhs,
                                                            value = patient.nhsNumbers.first()
                                                    ),
                                                    personName = PersonName(
                                                            name =
                                                            "${patient.name.title} " +
                                                                    "${patient.name.firstName} ${patient.name.surname}"
                                                    )
                                            )
                                    ),
                                    person =  mutableListOf()
                            )
                    )
        }
    }

    private fun generateVisionMocks(patient: Patient) {


        client.forVision.mock {
                    authentication.getRegisterRequest(
                            VisionMockDefaults.getVisionUserSession(patient),
                            patient)
                            .respondWithSuccess(
                                    Register(
                                            rosuAccountId = patient.rosuAccountId,
                                            apiToken = patient.apiKey)
                            )
                }

        client.forVision.mock {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId
                            ))
                            .respondWithSuccess(
                                    configuration = Configuration(account = Account(patient.patientId,
                                            patientNumber = listOf(PatientNumber(number = patient.nhsNumbers.first())),
                                            name = patient.formattedFullName()))
                            )
                }
    }
}
