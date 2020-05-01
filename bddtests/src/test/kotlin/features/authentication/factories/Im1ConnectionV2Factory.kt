package features.authentication.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import utils.SerenityHelpers
import worker.models.patient.Im1ConnectionRequest
import java.time.Duration

abstract class Im1ConnectionV2Factory(protected val gpSystem: Supplier) {

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

    abstract fun successfulIm1Register(linkageFacade: LinkageInformationFacade, delay: Duration? = null)

    abstract fun errorIm1Register(httpStatusCode: Int,
                                  errorCode: String,
                                  message: String? = null )

    val validLinkageDetails = LinkageInformationFacade(
            odsCode = patient.odsCode,
            linkageKey = patient.linkageKey,
            accountId = patient.accountId,
            nhsNumber = patient.nhsNumbers.first(),
            identityToken = identityToken,
            emailAddress = patient.contactDetails.emailAddress,
            surname = patient.name.surname,
            dateOfBirth = patient.age.dateOfBirth)

    val validIm1Request: Im1ConnectionRequest
            = Im1ConnectionRequest(
            AccountId = patient.accountId,
            LinkageKey = patient.linkageKey,
            OdsCode = patient.odsCode,
            Surname = patient.name.surname,
            DateOfBirth = patient.age.dateOfBirth)

    val validCreateLinkageRequest: Im1ConnectionRequest
            = Im1ConnectionRequest(
            OdsCode = patient.odsCode,
            Surname = patient.name.surname,
            DateOfBirth = patient.age.dateOfBirth,
            NhsNumber = patient.nhsNumbers.first(),
            IdentityToken = identityToken,
            EmailAddress = patient.contactDetails.emailAddress)

    companion object : SupplierSpecificFactory<Im1ConnectionV2Factory>() {

        override val map: HashMap<Supplier, () -> Im1ConnectionV2Factory>
            get() = hashMapOf(
                    Supplier.EMIS to { Im1ConnectionV2FactoryEmis() },
                    Supplier.TPP to { Im1ConnectionV2FactoryTpp() },
                    Supplier.VISION to { Im1ConnectionV2FactoryVision() },
                    Supplier.MICROTEST to { Im1ConnectionV2FactoryMicrotest() })
    }
}