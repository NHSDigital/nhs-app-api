package mocking.thirdPartyProviders.pkb

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class PKBRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/nhs-login/login") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun appointmentsRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/diary/listAppointments.action")
    )

    fun carePlanRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/auth/listPlans.action")
    )

    fun healthTrackerRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/pkbNhsMenu.action")
    )

    fun medicinesRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/auth/manageMedications.action?tab=treatments")
    )

    fun messagesRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/auth/getInbox.action?tab=messages")
    )

    fun sharedLinksRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/library/manageLibrary.action")
    )

    fun testResultsRequest() = PKBRequestBuilder(
            hashMapOf("phrPath" to "/test/myTests.action")
    )

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Patient Knows Best</title>
                    <body>
                        <h1>Patient Knows Best</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
