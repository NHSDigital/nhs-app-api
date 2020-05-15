package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingRouter
import mocking.favicon.FaviconMappingBuilder
import mocking.microtest.MicrotestMappingRouter
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

class MockingClient(configuration: MockingConfiguration) {

    val wiremockHelper = WiremockSetup(configuration)

    val forAzure = AzureMockingClient(wiremockHelper)

    val forCitizenId = ExternalSupplierMockingClient(CitizenIdMappingBuilder(), wiremockHelper)
    val forSpine = ExternalSupplierMockingClient(SpineMappingBuilder(), wiremockHelper)
    val forOrganDonation = ExternalSupplierMockingClient(OrganDonationMappingBuilder(), wiremockHelper)
    val forOnlineConsultations = ExternalSupplierMockingClient(OnlineConsultationsMappingBuilder(), wiremockHelper)
    val forNdop = ExternalSupplierMockingClient(NdopMappingBuilder(), wiremockHelper)
    val forMicrotest = ExternalSupplierMockingClient(MicrotestMappingRouter(), wiremockHelper)
    val forVision = ExternalSupplierMockingClient(VisionMappingRouter(), wiremockHelper)
    val forEmis = ExternalSupplierMockingClient(EmisMappingRouter(configuration.emisConfiguration), wiremockHelper)
    val forTpp = ExternalSupplierMockingClient(TppMappingRouter(), wiremockHelper)

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
