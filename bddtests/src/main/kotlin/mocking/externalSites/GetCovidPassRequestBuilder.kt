package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class GetCovidPassRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/covid-status-service-nhsx-nhs-uk") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>NHS COVID Pass - NHS</title>
                    <body>
                        <h1>Get your NHS COVID Pass</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
