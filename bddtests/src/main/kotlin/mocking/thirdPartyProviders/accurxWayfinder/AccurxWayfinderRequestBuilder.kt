package mocking.thirdPartyProviders.accurxWayfinder

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus
class AccurxWayfinderRequestBuilder : MappingBuilder("GET", "/api/OpenIdConnect/AuthenticateManageAppointment") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>AccurxWayfinder</title>
                    <body>
                        <h1>AccurxWayfinder</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
