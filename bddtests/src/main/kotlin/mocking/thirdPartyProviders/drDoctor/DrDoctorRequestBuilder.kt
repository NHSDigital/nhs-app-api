package mocking.thirdPartyProviders.drDoctor

import mocking.MappingBuilder
import mocking.WiremockUrlMatch
import mocking.models.Mapping
import org.apache.http.HttpStatus

class DrDoctorRequestBuilder : MappingBuilder("GET", "/appointments/(.*)", WiremockUrlMatch.UrlPattern) {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>DrDoctor</title>
                    <body>
                        <h1>DrDoctor</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
