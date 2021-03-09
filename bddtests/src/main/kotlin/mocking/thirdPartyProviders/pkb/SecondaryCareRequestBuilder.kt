package mocking.thirdPartyProviders.pkb

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class SecondaryCareRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/nhs-login/login") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun medicineRequest() = SecondaryCareRequestBuilder(
            hashMapOf("phrPath" to "/auth/manageMedications.action?tab=treatments",
                    "brand" to "pkbSecondaryCare")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Secondary Care</title>
                    <body>
                        <h1>Secondary Care</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
