package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.onlineConsultations.OnlineConsultationsMappingBuilder
import mocking.emis.EmisMappingRouter
import mocking.favicon.FaviconMappingBuilder
import mocking.microtest.MicrotestMappingRouter
import mocking.models.Mapping
import mocking.ndop.NdopMappingBuilder
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.spine.SpineMappingBuilder
import mocking.tpp.TppMappingRouter
import mocking.vision.VisionMappingRouter
import org.apache.http.HttpStatus
import org.hamcrest.Matchers.equalTo
import java.lang.Thread.sleep

private const val DEFAULT_REQUEST_ASSERT_DELAY_IN_MS = 500L

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

    fun forOrganDonation(method: String = "POST", resolver: OrganDonationMappingBuilder.() -> Mapping) {
        val mappingBuilder = OrganDonationMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forOnlineConsultations(method: String = "POST", resolver: OnlineConsultationsMappingBuilder.() -> Mapping) {
        val mappingBuilder = OnlineConsultationsMappingBuilder(method)
        val mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun favicon() = this.postMapping(FaviconMappingBuilder().respondWithNotFound())

    fun assertRequestWasMade(
        url: String,
        responseStatus: Int = HttpStatus.SC_OK,
        headers: Map<String, String>? = mapOf(),
        assertDelayInMs: Long = DEFAULT_REQUEST_ASSERT_DELAY_IN_MS) {
        var headersString = ""

        headers?.forEach { k, v ->
            headersString += "&& it.request.headers.$k == '$v' "
        }

        val groovyAssertion = "requests.any " +
            "{ it.request.url == '$url' $headersString" +
            "&& it.responseDefinition.status == $responseStatus }"

        sleep(assertDelayInMs)

        getRequests()
            .then()
            .assertThat()
            .body(groovyAssertion, equalTo(true))
    }

    companion object {
        val instance: MockingClient by lazy { EmisMockDefaults.createMockingClient(Config.instance) }
    }
}
