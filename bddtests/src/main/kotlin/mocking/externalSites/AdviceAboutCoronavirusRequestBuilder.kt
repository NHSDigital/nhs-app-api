package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class AdviceAboutCoronavirusRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/covid") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Advice About Coronavirus</title>
                    <body>
                        <h1>Advice About Coronavirus</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
