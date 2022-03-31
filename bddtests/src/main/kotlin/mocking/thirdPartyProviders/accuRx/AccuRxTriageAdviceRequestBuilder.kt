package mocking.thirdPartyProviders.accuRx

import mocking.models.Mapping
import org.apache.http.HttpStatus

class AccuRxTriageAdviceRequestBuilder
    : AccuRxRequestBuilder("GET", "/api/OpenIdConnect/AuthenticatePatientTriage") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>AccuRx</title>
                    <body>
                        <h1>AccuRx</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
