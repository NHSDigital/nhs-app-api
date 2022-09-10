package mocking.thirdPartyProviders.healthcarecomms

import mocking.MappingBuilder
import mocking.WiremockUrlMatch
import mocking.models.Mapping
import org.apache.http.HttpStatus

class HealthcareCommsRequestBuilder : MappingBuilder("GET", "/appointments/(.*)", WiremockUrlMatch.UrlPattern) {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>HealthcareComms</title>
                    <body>
                        <h1>HealthcareComms</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
