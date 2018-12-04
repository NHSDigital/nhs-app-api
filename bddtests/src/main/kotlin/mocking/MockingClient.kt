package mocking

import com.google.gson.Gson
import config.Config
import io.restassured.response.Response
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.EmisMockDefaults
import mocking.emis.EmisMappingBuilder
import mocking.favicon.FaviconMappingBuilder
import mocking.models.Mapping
import mocking.ndop.NdopMappingBuilder
import mocking.nhsAzureSearchService.NhsAzureSearchMappingBuilder
import mocking.throttling.BrotherMailerMappingBuilder
import mocking.throttling.BrotherMailerRedirectMappingBuilder
import mocking.tpp.TppMappingBuilder
import mocking.vision.VisionMappingBuilder
import net.serenitybdd.rest.SerenityRest
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_OK
import org.apache.http.client.methods.CloseableHttpResponse
import org.apache.http.client.methods.HttpDelete
import org.apache.http.impl.client.HttpClients

class MockingClient(private val configuration: MockingConfiguration) {

    private val gson = Gson()

    fun forEmis(method: String = "GET", resolver: EmisMappingBuilder.() -> Mapping) {
        val mappingBuilder = EmisMappingBuilder(configuration.emisConfiguration, method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forTpp(method: String = "POST", resolver: TppMappingBuilder.() -> Mapping) {
        val mappingBuilder = TppMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forCitizenId(method: String = "GET", resolver: CitizenIdMappingBuilder.() -> Mapping) {
        val mappingBuilder = CitizenIdMappingBuilder(method, "/emis")
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forVision(method: String = "POST", resolver: VisionMappingBuilder.() -> Mapping) {
        val mappingBuilder = VisionMappingBuilder(method)
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

    fun forBrotherMailerRedirect(method: String = "POST", resolver: BrotherMailerRedirectMappingBuilder.() -> Mapping) {
        val mappingBuilder = BrotherMailerRedirectMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }


    fun forNhsAzureSearch(method: String = "POST", resolver: NhsAzureSearchMappingBuilder.() -> Mapping) {
        val mappingBuilder = NhsAzureSearchMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun favicon() = this.postMapping(FaviconMappingBuilder().respondWithNotFound())

    private fun postMapping(mapping: Mapping): Response {
        return SerenityRest.given()
                .header("Content-Type", "application/json; charset=UTF-8")
                .and()
                .body(gson.toJson(mapping))
                .expect().statusCode(HttpStatus.SC_CREATED)
                .`when`()
                .post("${configuration.wiremockAdminUrl}/mappings")
    }

    fun clearWiremock() {
        deleteWiremockDetails(endpoint = "mappings")
        deleteWiremockDetails(endpoint = "requests")
    }

    private fun deleteWiremockDetails(endpoint: String) {
        val uri = "${configuration.wiremockAdminUrl}/$endpoint"
        val httpDelete = HttpDelete(uri)
        val response = HttpClients.createDefault().execute(httpDelete)
        if (response.statusLine.statusCode != SC_OK) {
            reportWiremockError(response)
            throw IllegalStateException("Failed to delete mappings using URI: $uri")
        }

        httpDelete.releaseConnection()
    }

    private fun reportWiremockError(response: CloseableHttpResponse) {
        println("Call to wiremock failed:")
        println("  Status code: ${response.statusLine.statusCode}")
        println("  Reason:      ${response.statusLine.reasonPhrase}")
    }

    companion object {
        val instance: MockingClient by lazy { EmisMockDefaults.createMockingClient(Config.instance) }
    }
}
