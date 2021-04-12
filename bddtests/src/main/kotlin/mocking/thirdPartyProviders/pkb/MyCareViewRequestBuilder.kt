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

    fun appointmentRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/diary/listAppointments.action",
                    "brand" to "pkbMyCareView")
    )

    fun carePlanRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/auth/listPlans.action",
                    "brand" to "pkbMyCareView")
    )

    fun messagesRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/auth/getInbox.action?tab=messages",
                    "brand" to "pkbMyCareView")
    )

    fun medicinesRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/auth/manageMedications.action?tab=treatments",
                    "brand" to "pkbMyCareView")
    )

    fun recordSharingRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/patient/myConsentTeam.action?tab=invitations",
                    "subTab" to "myClinicians",
                    "brand" to "pkbMyCareView")
    )

    fun sharedLinksRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/library/manageLibrary.action",
                    "brand" to "pkbMyCareView")
    )

    fun testResultsRequest() = MyCareViewRequestBuilder(
            hashMapOf("phrPath" to "/test/myTests.action",
                    "brand" to "pkbMyCareView")
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
