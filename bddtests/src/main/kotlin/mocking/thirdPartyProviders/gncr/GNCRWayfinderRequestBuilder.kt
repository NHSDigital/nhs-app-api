package mocking.thirdPartyProviders.gncr

import mocking.MappingBuilder
import mocking.WiremockUrlMatch
import mocking.models.Mapping
import org.apache.http.HttpStatus

class GNCRWayfinderRequestBuilder
    : MappingBuilder("GET", "/Appointment/(.*)", WiremockUrlMatch.UrlPattern) {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>GNCRWayfinder</title>
                    <body>
                        <h1>GNCRWayfinder</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
