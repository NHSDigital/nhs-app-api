package mocking.thirdPartyProviders.engage

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EngageRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun adminRequest() = EngageRequestBuilder(
            hashMapOf("sso_route" to "admin")
    )

    fun medicalAdviceRequest() = EngageRequestBuilder(
            hashMapOf("sso_route" to "medical")
    )

    fun messagesRequest() = EngageRequestBuilder(
            hashMapOf("sso_route" to "messages")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Engage</title>
                    <body>
                        <h1>Engage</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
