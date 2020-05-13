package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingRouter
import mocking.favicon.FaviconMappingBuilder
import mocking.microtest.MicrotestMappingRouter
import mocking.models.Mapping
import mocking.ndop.NdopMappingBuilder
import mocking.onlineConsultations.OnlineConsultationsMappingBuilder
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.spine.SpineMappingBuilder
import mocking.tpp.TppMappingRouter
import mocking.vision.VisionMappingRouter
import org.apache.http.HttpStatus
import org.hamcrest.Matchers.equalTo
import java.lang.Thread.sleep

private const val DEFAULT_REQUEST_ASSERT_DELAY_IN_MS = 500L

class MockingClient(private val configuration: MockingConfiguration) {

    val wiremockHelper = WiremockHelper(configuration)

    val forAzure = AzureMockingClient(wiremockHelper)

    val forCitizenId = ExternalSupplierMockingClient(CitizenIdMappingBuilder(), wiremockHelper)
    val forSpine = ExternalSupplierMockingClient(SpineMappingBuilder(), wiremockHelper)
    val forOrganDonation = ExternalSupplierMockingClient(OrganDonationMappingBuilder(), wiremockHelper)
    val forOnlineConsultations = ExternalSupplierMockingClient(OnlineConsultationsMappingBuilder(), wiremockHelper)
    val forNdop = ExternalSupplierMockingClient(NdopMappingBuilder(), wiremockHelper)

    fun forEmis(resolver: EmisMappingRouter.() -> Mapping) {
        val router = EmisMappingRouter(configuration.emisConfiguration)
        val mapping: Mapping = router.resolver()

        wiremockHelper.postMapping(mapping)
    }

    fun forTpp(resolver: TppMappingRouter.() -> Mapping) {
        val router = TppMappingRouter()
        val mapping: Mapping = router.resolver()

        wiremockHelper.postMapping(mapping)
    }

    fun forVision(resolver: VisionMappingRouter.() -> Mapping) {
        val router = VisionMappingRouter()
        val mapping: Mapping = router.resolver()

        wiremockHelper.postMapping(mapping)
    }

    fun forMicrotest(resolver: MicrotestMappingRouter.() -> Mapping) {
        val router = MicrotestMappingRouter()
        val mapping: Mapping = router.resolver()

        wiremockHelper.postMapping(mapping)
    }

    fun favicon() = wiremockHelper.postMapping(FaviconMappingBuilder().respondWithNotFound())

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

        wiremockHelper.getRequests()
            .then()
            .assertThat()
            .body(groovyAssertion, equalTo(true))
    }

    companion object {
        val instance: MockingClient by lazy { EmisMockDefaults.createMockingClient(Config.instance) }
    }
}
