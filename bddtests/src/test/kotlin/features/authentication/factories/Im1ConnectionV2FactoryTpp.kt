package features.authentication.factories

import constants.DateTimeFormats
import constants.TppConstants
import mocking.defaults.TppMockDefaults
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import mocking.tpp.models.LinkAccount
import mocking.tpp.models.LinkAccountReply
import mocking.tpp.models.NationalId
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.User
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient

class Im1ConnectionV2FactoryTpp:  Im1ConnectionV2Factory("TPP") {

    override fun successfulIm1Register(linkageFacade: LinkageInformationFacade) {
        authenticate()
        mockingClient.forTpp {
            authentication.linkAccountRequest(patient).respondWithSuccess(
                    LinkAccountReply(
                            passphrase = patient.passphrase,
                            uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                            passphraseToLink = "passphraseToLink"
                    ))
        }
    }

    override fun errorIm1Register(httpStatusCode: Int, errorCode: String, message: String?) {

        authenticate()
        val error = Error(
                errorCode,
                "Mocked TPP Error"
        )

        mockingClient.forTpp {
            authentication.linkAccountRequest(patient).respondWithError(error, httpStatusCode
            )
        }
    }

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone


    override fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)->    Mapping) {
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))
        mockingClient.forTpp {
            action( authentication.linkageKeyPOSTRequest(linkAccount))
        }
    }

    override fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade) {
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))
        mockingClient.forTpp {
            authentication.linkageKeyPOSTRequest(linkAccount).respondWithSuccessfullyCreated(linkageInformationFacade)
        }
    }

    override fun linkageGet(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)->    Mapping) {
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))
        mockingClient.forTpp {
            action( authentication.linkageKeyGetRequest(linkAccount))
        }
    }

    override fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade) {
        //There's no such thing as a 'successful' get which will retrieve linkage details.
        //TPP is hardcoded to always return 'NotFound'
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))

        mockingClient.forTpp {
            authentication.linkageKeyGetRequest(linkAccount).respondWithNotFound()
        }
    }

    private fun authenticate() {
        mockingClient.forTpp {
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
                                                    dateOfBirth = patient.dateOfBirth,
                                                    gender = patient.sex.name,
                                                    nationalId = NationalId(
                                                            type = TppConstants.NationalIdTypeNhs,
                                                            value = patient.nhsNumbers.first()
                                                    ),
                                                    personName = PersonName(
                                                            name =
                                                            "${patient.title} " +
                                                                    "${patient.firstName} ${patient.surname}"
                                                    )
                                            )
                                    )
                            )
                    )
        }
    }
}
