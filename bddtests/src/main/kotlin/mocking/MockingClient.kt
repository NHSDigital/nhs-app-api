package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingRouter
import mocking.favicon.FaviconMappingBuilder
import mocking.microtest.MicrotestMappingRouter
import mocking.models.Mapping
import mocking.ndop.NdopMappingBuilder
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.spine.SpineMappingBuilder
import mocking.throttling.BrotherMailerMappingBuilder
import mocking.tpp.TppMappingRouter
import mocking.vision.VisionMappingRouter

class MockingClient(configuration: MockingConfiguration): WiremockHelper(configuration) {

    var forAzure = AzureMockingClient(configuration)

    fun forCitizenId(method: String = "GET", resolver: CitizenIdMappingBuilder.() -> Mapping) {
        val mappingBuilder = CitizenIdMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forEmis(resolver: EmisMappingRouter.() -> Mapping) {
        val router = EmisMappingRouter(configuration.emisConfiguration)
        val mapping: Mapping = router.resolver()

        this.postMapping(mapping)
    }

    fun forSpine(method: String = "GET", resolver: SpineMappingBuilder.() -> Mapping) {
        val mappingBuilder = SpineMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forTpp(resolver: TppMappingRouter.() -> Mapping) {
        val router = TppMappingRouter()
        val mapping: Mapping = router.resolver()

        this.postMapping(mapping)
    }

    fun forVision(resolver: VisionMappingRouter.() -> Mapping) {
        val router = VisionMappingRouter()
        val mapping: Mapping = router.resolver()

        this.postMapping(mapping)
    }

    fun forMicrotest(resolver: MicrotestMappingRouter.() -> Mapping) {
        val router = MicrotestMappingRouter()
        val mapping: Mapping = router.resolver()

        this.postMapping(mapping)
    }

    fun forNdop(method: String = "POST", resolver: NdopMappingBuilder.() -> Mapping) {
        val mappingBuilder = NdopMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forBrotherMailer(method: String = "POST", resolver: BrotherMailerMappingBuilder.() -> Mapping) {
        val mappingBuilder = BrotherMailerMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forOrganDonation(method: String = "POST", resolver: OrganDonationMappingBuilder.() -> Mapping) {
        val mappingBuilder = OrganDonationMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun favicon() = this.postMapping(FaviconMappingBuilder().respondWithNotFound())

    companion object {
        val instance: MockingClient by lazy { EmisMockDefaults.createMockingClient(Config.instance) }
    }
}
