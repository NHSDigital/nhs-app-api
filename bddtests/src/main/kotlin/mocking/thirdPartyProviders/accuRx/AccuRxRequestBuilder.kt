package mocking.thirdPartyProviders.accuRx

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class AccuRxRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/api/OpenIdConnect/AuthenticatePatientTriage") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun messagesRequest() = AccuRxRequestBuilder(
        hashMapOf("requestType" to "admin")
    )

    fun medicalAdviceRequest() = AccuRxRequestBuilder(
        hashMapOf("requestType" to "medical")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>AccuRx</title>
                    <body>
                        <h1>AccuRx</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
