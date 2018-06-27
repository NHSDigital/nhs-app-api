package mocking

import com.google.gson.Gson
import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.MockDefaults
import mocking.emis.EmisMappingBuilder
import mocking.favicon.FaviconMappingBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import net.serenitybdd.core.Serenity
import net.serenitybdd.rest.SerenityRest
import org.apache.http.HttpException
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_OK
import org.apache.http.client.methods.CloseableHttpResponse
import org.apache.http.client.methods.HttpDelete
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.ContentType
import org.apache.http.entity.StringEntity
import org.apache.http.impl.client.HttpClients
import java.io.BufferedReader
import java.io.InputStreamReader

class MockingClient(private val configuration: MockingConfiguration) {

    private val gson = Gson()

    fun forEmis(method: String = "GET", resolver: EmisMappingBuilder.() -> Mapping) {
        val mappingBuilder = EmisMappingBuilder(configuration.emisConfiguration, method, "/emis")
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forTpp(method: String = "POST", resolver: TppMappingBuilder.() -> Mapping) {
        val mappingBuilder = TppMappingBuilder(method, "/tpp")
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forCitizenId(method: String = "GET", resolver: CitizenIdMappingBuilder.() -> Mapping) {
        val mappingBuilder = CitizenIdMappingBuilder(method, "/emis")
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun favicon() = this.postMapping(FaviconMappingBuilder().respondWithNotFound())

    private fun postMapping(mapping: Mapping): String {
        val response = SerenityRest.given()
                .header("Content-Type", "application/json; charset=UTF-8")
                .and()
                .body(gson.toJson(mapping))
                .expect().statusCode(HttpStatus.SC_CREATED)
                .`when`()
                .post("${configuration.wiremockAdminUrl}/mappings")

        return response.body.prettyPrint()
    }

    fun getRequests(): String {
        val getMethod = HttpGet("${configuration.wiremockAdminUrl}/requests")
        val response = HttpClients.createDefault().execute(getMethod)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        getMethod.releaseConnection()
        return result
    }

    fun clearWiremock() {
        deleteWiremockDetails(endpoint = "mappings")
        deleteWiremockDetails(endpoint = "requests")
    }

    fun resetScenarios() {
        val httpPost = HttpPost("${configuration.wiremockAdminUrl}/scenarios/reset/")

        val response = HttpClients.createDefault().execute(httpPost).also { println("Resetting Scenarios... Response: $it") }
        httpPost.releaseConnection()

        if (response.statusLine.statusCode != HttpStatus.SC_OK) {
            throw HttpException(String.format("Resetting Scenarios failed, response was %1\$s: %2\$s", response.statusLine.statusCode, response.toString()))
        }
    }

    private fun deleteWiremockDetails(endpoint: String) {
        val uri = "${configuration.wiremockAdminUrl}/$endpoint"
        val httpDelete = HttpDelete(uri)
        val response = HttpClients.createDefault().execute(httpDelete)
        if (response.statusLine.statusCode != SC_OK) {
            reportWiremockError(response)
            throw Exception("Failed to delete mappings using URI: $uri")
        }

        httpDelete.releaseConnection()
    }

    private fun reportWiremockError(response: CloseableHttpResponse) {
        println("Call to wiremock failed:")
        println("  Status code: ${response.statusLine.statusCode}")
        println("  Reason:      ${response.statusLine.reasonPhrase}")
    }

    companion object {
        val instance: MockingClient by lazy { MockDefaults.createMockingClient(Config.instance) }
    }
    
}
