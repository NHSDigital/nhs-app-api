package mocking.thirdPartyProviders.pkb

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class CIERequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/nhs-login/login") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun appointmentsRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/diary/listAppointments.action",
                    "brand" to "cie")
    )

    fun carePlanRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/auth/listPlans.action",
                    "brand" to "cie")
    )

    fun healthTrackerRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/pkbNhsMenu.action",
                    "brand" to "cie")
    )

    fun medicinesRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/auth/manageMedications.action?tab=treatments",
                    "brand" to "cie")
    )

    fun messagesRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/auth/getInbox.action?tab=messages",
                    "brand" to "cie")
    )

    fun sharedLinksRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/library/manageLibrary.action",
                    "brand" to "cie")
    )

    fun testResultsRequest() = CIERequestBuilder(
            hashMapOf("phrPath" to "/test/myTests.action",
                    "brand" to "cie")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Care Information Exchange</title>
                    <body>
                        <h1>Care Information Exchange</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
