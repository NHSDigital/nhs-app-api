package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class OneOneOneOnlineRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/111") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>111 online</title>
                    <body>
                        <h1>111 online</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
