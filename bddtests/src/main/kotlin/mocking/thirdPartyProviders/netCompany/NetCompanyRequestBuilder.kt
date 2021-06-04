package mocking.thirdPartyProviders.netCompany

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class NetCompanyRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/covid-status-sso") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun vaccineRecordRequest() = NetCompanyRequestBuilder()

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Net Company</title>
                    <body>
                        <h1>Net Company</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
