package features.linkage.stepDefinitions

import features.linkage.LinkageResult
import utils.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import org.junit.Assert

abstract class LinkageFactory(protected val gpSystem: String) {

    protected val mockingClient = SerenityHelpers.getMockingClient()

    protected val patient = Patient.getDefault(gpSystem)

    abstract val linkageDateOfBirthFormat: String

    abstract fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade, linkageResult: LinkageResult)

    abstract fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade, linkageResult: LinkageResult)

    abstract val validLinkageDetails: LinkageInformationFacade

    protected fun <TMapping>responseFromMap(linkageToGetRequestResponse: HashMap<LinkageResult, TMapping?>,
                                          linkageResult: LinkageResult): TMapping?{

        Assert.assertTrue("Test Setup Incorrect, Mapping not set up for linkage $linkageResult",
                linkageToGetRequestResponse.containsKey(linkageResult))

        return linkageToGetRequestResponse[linkageResult]
    }

    companion object : SupplierSpecificFactory<LinkageFactory>() {

        override val map: HashMap<String, () -> LinkageFactory>
            get() = hashMapOf(
                    "EMIS" to {LinkageFactoryEmis()},
                    "TPP" to {LinkageFactoryTpp()},
                    "VISION" to {LinkageFactoryVision()})
    }
}
