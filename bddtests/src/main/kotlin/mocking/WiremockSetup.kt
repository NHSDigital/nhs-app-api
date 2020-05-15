package mocking

import com.google.gson.Gson
import io.restassured.response.Response
import mocking.models.Mapping
import net.serenitybdd.rest.SerenityRest
import org.apache.http.HttpStatus
import org.apache.http.client.methods.CloseableHttpResponse
import org.apache.http.client.methods.HttpDelete
import org.apache.http.impl.client.HttpClients

class WiremockSetup(val configuration: MockingConfiguration) {

    private val gson = Gson()

    fun postMapping(mapping: Mapping): Response {
        return SerenityRest.given()
                .header("Content-Type", "application/json; charset=UTF-8")
                .and()
                .body(gson.toJson(mapping))
                .expect()
                .statusCode(HttpStatus.SC_CREATED)
                .`when`()
                .post("${configuration.wiremockAdminUrl}/mappings")
    }

    fun clearWiremock() {
        deleteWiremockDetails(endpoint = "mappings")
        deleteWiremockDetails(endpoint = "requests")
    }

    fun getRequests(): Response {
        return SerenityRest
            .expect()
            .statusCode(HttpStatus.SC_OK)
            .`when`()
            .get("${configuration.wiremockAdminUrl}/requests")
    }

    private fun deleteWiremockDetails(endpoint: String) {
        val uri = "${configuration.wiremockAdminUrl}/$endpoint"
        val httpDelete = HttpDelete(uri)
        val response = HttpClients.createDefault().execute(httpDelete)
        if (response.statusLine.statusCode != HttpStatus.SC_OK) {
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
}