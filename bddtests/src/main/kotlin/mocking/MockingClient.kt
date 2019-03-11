package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingBuilder
import mocking.favicon.FaviconMappingBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mocking.ndop.NdopMappingBuilder
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationMappingBuilder
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesAndPlacesMappingBuilder
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.spine.SpineMappingBuilder
import mocking.throttling.BrotherMailerMappingBuilder
import mocking.tpp.TppMappingBuilder
import mocking.vision.VisionMappingBuilder

@Suppress("TooManyFunctions")
class MockingClient(configuration: MockingConfiguration): WiremockHelper(configuration) {

    fun forCitizenId(method: String = "GET", resolver: CitizenIdMappingBuilder.() -> Mapping) {
        val mappingBuilder = CitizenIdMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forEmis(method: String = "GET", resolver: EmisMappingBuilder.() -> Mapping) {
        val mappingBuilder = EmisMappingBuilder(configuration.emisConfiguration, method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forSpine(method: String = "GET", resolver: SpineMappingBuilder.() -> Mapping) {
        val mappingBuilder = SpineMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forTpp(method: String = "POST", resolver: TppMappingBuilder.() -> Mapping) {
        val mappingBuilder = TppMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forVision(method: String = "POST", resolver: VisionMappingBuilder.() -> Mapping) {
        val mappingBuilder = VisionMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forMicrotest(method: String = "GET", resolver: MicrotestMappingBuilder.() -> Mapping) {
        val mappingBuilder = MicrotestMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

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

    fun forNhsAzureSearchOrganisation(method: String = "POST", resolver:
    NhsAzureSearchOrganisationMappingBuilder.()
    -> Mapping) {
        val mappingBuilder = NhsAzureSearchOrganisationMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forNhsAzureSearchPostcodesAndPlaces(method: String = "POST", resolver:
    NhsAzureSearchPostcodesAndPlacesMappingBuilder.()
    -> Mapping) {
        val mappingBuilder = NhsAzureSearchPostcodesAndPlacesMappingBuilder(method)
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
