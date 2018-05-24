package mocking

import com.google.gson.Gson
import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.emis.EmisMappingBuilder
import mocking.favicon.FaviconMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpException
import org.apache.http.HttpStatus
import org.apache.http.client.methods.HttpDelete
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.ContentType
import org.apache.http.entity.StringEntity
import org.apache.http.impl.client.HttpClients

class MockingClient(private val configuration: MockingConfiguration) {

    private val gson = Gson()

    fun forEmis(method: String = "GET", resolver: EmisMappingBuilder.() -> Mapping) {
        val mappingBuilder = EmisMappingBuilder(configuration.emisConfiguration, method, "/emis")
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
        val httpPost = HttpPost(configuration.wiremockBaseUrl + "/__admin/mappings")

        httpPost.addHeader("Content-Type", "application/json; charset=UTF-8")
        httpPost.entity = StringEntity(gson.toJson(mapping), ContentType.APPLICATION_JSON)

        val response = HttpClients.createDefault().execute(httpPost).also { println("Posting $mapping... Response: $it") }
        httpPost.releaseConnection()

        if (response.statusLine.statusCode != HttpStatus.SC_CREATED) {
            throw HttpException(String.format("PostMapping failed, response was %1\$s: %2\$s", response.statusLine.statusCode, response.toString()))
        }
        return response.toString()
    }

    fun resetWiremock() {
        resetWiremockDetails("mappings")
        resetWiremockDetails("requests")
    }

    private fun resetWiremockDetails(endpoint: String) {
        val httpDelete = HttpDelete(configuration.wiremockBaseUrl + endpoint)
        HttpClients.createDefault().execute(httpDelete)
        httpDelete.releaseConnection()
    }

    companion object {
        val instance: MockingClient by lazy { MockDefaults.createMockingClient(Config.instance) }
    }

}
