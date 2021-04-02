package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class BloodDonorRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/blood-donor") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Blood Donor</title>
                    <body>
                        <h1>Blood Donor</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
