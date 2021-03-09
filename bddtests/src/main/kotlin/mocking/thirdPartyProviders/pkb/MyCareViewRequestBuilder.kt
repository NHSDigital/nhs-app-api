package mocking.thirdPartyProviders.pkb

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class MyCareViewRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/nhs-login/login") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun appointmentsRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/auth/manageMedications.action?tab=treatments",
                    "brand" to "myCareView")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>My Care View</title>
                    <body>
                        <h1>My Care View</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
