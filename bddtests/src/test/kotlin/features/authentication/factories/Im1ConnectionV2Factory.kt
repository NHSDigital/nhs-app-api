package features.authentication.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import utils.SerenityHelpers
import worker.models.patient.Im1ConnectionRequest

abstract class Im1ConnectionV2Factory(protected val gpSystem: String) {

    protected val mockingClient = SerenityHelpers.getMockingClient()

    protected val identityToken = "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc"

    val patient = Patient.getDefault(gpSystem)

    abstract val linkageDateOfBirthFormat: String

    abstract fun linkageGet(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)-> Mapping)

    abstract fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                             action: (IErrorMappingBuilder)-> Mapping)

    abstract fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade)

    abstract fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade)

    abstract fun successfulIm1Register(linkageFacade: LinkageInformationFacade)

    abstract fun errorIm1Register(httpStatusCode: Int,
                                  errorCode: String,
                                  message: String? = null )

    val validLinkageDetails = LinkageInformationFacade(
            odsCode = patient.odsCode,
            linkageKey = patient.linkageKey,
            accountId = patient.accountId,
            nhsNumber = patient.nhsNumbers.first(),
            identityToken = identityToken,
            emailAddress = patient.emailAddress,
            surname = patient.surname,
            dateOfBirth = patient.dateOfBirth)

    val validIm1Request: Im1ConnectionRequest
            = Im1ConnectionRequest(
            AccountId = patient.accountId,
            LinkageKey = patient.linkageKey,
            OdsCode = patient.odsCode,
            Surname = patient.surname,
            DateOfBirth = patient.dateOfBirth,
            NhsNumber = patient.nhsNumbers.first(),
            IdentityToken = identityToken,
            EmailAddress = patient.emailAddress)

    companion object : SupplierSpecificFactory<Im1ConnectionV2Factory>() {

        override val map: HashMap<String, () -> Im1ConnectionV2Factory>
            get() = hashMapOf(
                    "EMIS" to { Im1ConnectionV2FactoryEmis() },
                    "TPP" to { Im1ConnectionV2FactoryTpp() },
                    "VISION" to { Im1ConnectionV2FactoryVision() },
                    "MICROTEST" to { Im1ConnectionV2FactoryMicrotest() })
    }
}