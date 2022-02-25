package mocking.thirdPartyProviders.wellnessAndPrevention

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class WellnessAndPreventionRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/sso") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Wellness and Prevention</title>
                    <body>
                        <h1>Wellness and Prevention</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
