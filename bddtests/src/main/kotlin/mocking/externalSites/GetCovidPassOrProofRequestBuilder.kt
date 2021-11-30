package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class GetCovidPassOrProofRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/conditions/coronavirus-covid-19/covid-pass") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>NHS COVID Pass - NHS</title>
                    <body>
                        <h1>NHS COVID Pass</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
