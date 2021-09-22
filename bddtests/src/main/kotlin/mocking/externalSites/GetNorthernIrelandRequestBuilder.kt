package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class GetNorthernIrelandRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/www-nidirect-gov-uk/articles/gp-out-hours-service") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>GP out of hours service | nidirect</title>
                    <body>
                        <h1>Search n i direct</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
