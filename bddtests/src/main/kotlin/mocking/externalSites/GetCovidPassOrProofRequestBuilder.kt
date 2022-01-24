package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class GetCovidPassOrProofRequestBuilder(method: String ="GET", relativePath:String="")
    : ExternalSitesMappingBuilder(method, "/conditions/coronavirus-covid-19/covid-pass$relativePath") {

    fun respondWithPage(serviceName: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>$serviceName</title>
                    <body>
                        <h1>$serviceName</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
