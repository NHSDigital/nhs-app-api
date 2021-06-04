package mocking.thirdPartyProviders.nhsd

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class NhsdRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/sso") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun vaccineRecordRequest() = NhsdRequestBuilder()

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Nhsd</title>
                    <body>
                        <h1>Nhsd</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
