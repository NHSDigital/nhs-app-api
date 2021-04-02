package mocking.externalSites

import mocking.models.Mapping
import org.apache.http.HttpStatus

class InformaticaRequestBuilder
    : ExternalSitesMappingBuilder("GET", "/andover-medical-practice.appointments-online.co.uk/") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Informatica</title>
                    <body>
                        <h1>Informatica</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
