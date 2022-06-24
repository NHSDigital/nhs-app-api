package mocking.thirdPartyProviders.accurx

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class AccurxRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/api/OpenIdConnect/AuthenticatePatientTriage") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun messagesRequest() = AccurxRequestBuilder(
        hashMapOf("requestType" to "admin")
    )

    fun medicalAdviceRequest() = AccurxRequestBuilder(
        hashMapOf("requestType" to "medical")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Accurx</title>
                    <body>
                        <h1>Accurx</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
