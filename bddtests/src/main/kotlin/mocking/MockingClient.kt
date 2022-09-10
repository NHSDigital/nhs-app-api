package mocking

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingRouter
import mocking.externalSites.ExternalSitesMappingBuilder
import mocking.favicon.FaviconMappingBuilder
import mocking.help.HelpRequestBuilder
import mocking.ndop.NdopMappingBuilder
import mocking.onlineConsultations.OnlineConsultationsMappingBuilder
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.qualtrics.QualtricsMappingBuilder
import mocking.spine.SpineMappingBuilder
import mocking.thirdPartyProviders.accurx.AccurxRequestBuilder
import mocking.thirdPartyProviders.accurxWayfinder.AccurxWayfinderRequestBuilder
import mocking.thirdPartyProviders.drDoctor.DrDoctorRequestBuilder
import mocking.thirdPartyProviders.engage.EngageRequestBuilder
import mocking.thirdPartyProviders.healthcarecomms.HealthcareCommsRequestBuilder
import mocking.thirdPartyProviders.gncr.GNCRRequestBuilder
import mocking.thirdPartyProviders.netCompany.NetCompanyRequestBuilder
import mocking.thirdPartyProviders.netcall.NetcallRequestBuilder
import mocking.thirdPartyProviders.nhsd.NhsdRequestBuilder
import mocking.thirdPartyProviders.pkb.PKBRequestBuilder
import mocking.thirdPartyProviders.substrakt.SubstraktRequestBuilder
import mocking.thirdPartyProviders.wellnessAndPrevention.WellnessAndPreventionRequestBuilder
import mocking.thirdPartyProviders.zesty.ZestyRequestBuilder
import mocking.tpp.TppMappingRouter
import mocking.vision.VisionMappingRouter
import mocking.wayfinder.WayfinderMappingBuilder
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
    val forQualtrics = ExternalSupplierMockingClient(QualtricsMappingBuilder(), wiremockHelper)

    val forVision = ExternalSupplierMockingClient(VisionMappingRouter(), wiremockHelper)
    val forEmis = ExternalSupplierMockingClient(EmisMappingRouter(configuration.emisConfiguration), wiremockHelper)
    val forTpp = ExternalSupplierMockingClient(TppMappingRouter(), wiremockHelper)

    val forExternalSites = ExternalSupplierMockingClient(ExternalSitesMappingBuilder(), wiremockHelper)

    val forAccurx = ExternalSupplierMockingClient(AccurxRequestBuilder(), wiremockHelper)
    val forAccurxWayfinder = ExternalSupplierMockingClient(AccurxWayfinderRequestBuilder(), wiremockHelper)
    val forDrDoctor = ExternalSupplierMockingClient(DrDoctorRequestBuilder(), wiremockHelper)
    val forEngage = ExternalSupplierMockingClient(EngageRequestBuilder(), wiremockHelper)
    val forHealthcareComms = ExternalSupplierMockingClient(HealthcareCommsRequestBuilder(), wiremockHelper)    
    val forGNCR = ExternalSupplierMockingClient(GNCRRequestBuilder(), wiremockHelper)
    val forNetcall = ExternalSupplierMockingClient(NetcallRequestBuilder(), wiremockHelper)
    val forNetCompany = ExternalSupplierMockingClient(NetCompanyRequestBuilder(), wiremockHelper)
    val forNhsd = ExternalSupplierMockingClient(NhsdRequestBuilder(), wiremockHelper)
    val forPKB = ExternalSupplierMockingClient(PKBRequestBuilder(), wiremockHelper)
    val forSubstrakt = ExternalSupplierMockingClient(SubstraktRequestBuilder(), wiremockHelper)
    val forWellness = ExternalSupplierMockingClient(WellnessAndPreventionRequestBuilder(), wiremockHelper)
    val forZesty = ExternalSupplierMockingClient(ZestyRequestBuilder(), wiremockHelper)

    val forWayfinder = ExternalSupplierMockingClient(WayfinderMappingBuilder(), wiremockHelper)

    val forHelp = ExternalSupplierMockingClient(HelpRequestBuilder(), wiremockHelper)

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
