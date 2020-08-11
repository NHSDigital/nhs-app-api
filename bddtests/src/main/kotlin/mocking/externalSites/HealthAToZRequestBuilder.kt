package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class HealthAToZRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/healthAtoZ") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Health A to Z</title>
                    <body>
                        <h1>Health A to Z</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
