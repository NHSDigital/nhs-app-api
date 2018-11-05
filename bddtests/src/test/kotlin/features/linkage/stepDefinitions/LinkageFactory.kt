package features.linkage.stepDefinitions

import features.linkage.LinkageResult
import utils.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mockingFacade.linkage.LinkageInformationFacade
import org.junit.Assert
import java.util.*

abstract class LinkageFactory(protected val gpSystem: String) {

    protected val mockingClient = SerenityHelpers.getMockingClient()

    abstract fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade, linkageResult: LinkageResult)

    abstract fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade, linkageResult: LinkageResult)

    protected fun <TMapping>responseFromMap(linkageToGetRequestResponse: HashMap<LinkageResult, TMapping?>,
                                          linkageResult: LinkageResult): TMapping?{

        Assert.assertTrue("Test Setup Incorrect, Mapping not set up for linkage $linkageResult",
                linkageToGetRequestResponse.containsKey(linkageResult))

        return linkageToGetRequestResponse[linkageResult]
    }

    companion object : SupplierSpecificFactory<LinkageFactory>() {

        override val map: HashMap<String, () -> LinkageFactory>
            get() = hashMapOf("EMIS" to {LinkageFactoryEmis()})

    }
}

