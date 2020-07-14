package features.linkage.stepDefinitions

import constants.Supplier
import features.linkage.LinkageResult
import mocking.SupplierSpecificFactory
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers

abstract class LinkageFactory(protected val gpSystem: Supplier) {

    protected val mockingClient = SerenityHelpers.getMockingClient()

    protected val patient = Patient.getDefault(gpSystem)

    abstract val linkageDateOfBirthFormat: String

    abstract fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade, linkageResult: LinkageResult)

    abstract fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade,
                                       linkageResult: LinkageResult,
                                       delay: Long?)

    abstract val validLinkageDetails: LinkageInformationFacade

    abstract val validOtherLinkageDetails: LinkageInformationFacade

    protected fun <TMapping> responseFromMap(linkageToGetRequestResponse: HashMap<LinkageResult, TMapping>,
                                             linkageResult: LinkageResult): TMapping? {

        Assert.assertTrue("Test Setup Incorrect, Mapping not set up for linkage $linkageResult",
                linkageToGetRequestResponse.containsKey(linkageResult))

        return linkageToGetRequestResponse[linkageResult]
    }

    companion object : SupplierSpecificFactory<LinkageFactory>() {

        fun setUpPostMocks(delay: Long? = null): LinkageInformationFacade {
            val gpSystem = SerenityHelpers.getGpSupplier()
            val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
            val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
            // Only setup mock for gp supplier if we need to.
            // Some requests testing validation don't get as far as calling a gp supplier.
            if (linkage != null && linkageResult != null) {
                getForSupplier(gpSystem).mockLinkagePostResult(linkage, linkageResult, delay)
            }
            return linkage
        }

        override val map: HashMap<Supplier, () -> LinkageFactory>
            get() = hashMapOf(
                    Supplier.EMIS to { LinkageFactoryEmis() },
                    Supplier.TPP to { LinkageFactoryTpp() },
                    Supplier.VISION to { LinkageFactoryVision() },
                    Supplier.MICROTEST to { LinkageFactoryMicrotest() })

        fun validLinkage(gpSystem: Supplier): LinkageInformationFacade {
            SerenityHelpers.setPatient(Patient.getDefault(gpSystem))
            SerenityHelpers.setGpSupplier(gpSystem)
            return LinkageFactory.getForSupplier(gpSystem).validLinkageDetails
        }

        fun validOtherLinkage(gpSystem: Supplier): LinkageInformationFacade {
            SerenityHelpers.setGpSupplier(gpSystem)
            return LinkageFactory.getForSupplier(gpSystem).validOtherLinkageDetails
        }

        fun setLinkageInformation(linkageInformationFacade: LinkageInformationFacade,
                                          linkageResult: LinkageResult) {
            Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkageInformationFacade)
            Serenity.setSessionVariable(LinkageResult::class).to(linkageResult)
        }
    }
}
