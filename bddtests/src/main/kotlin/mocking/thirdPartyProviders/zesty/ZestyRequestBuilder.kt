package mocking.thirdPartyProviders.zesty

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class ZestyRequestBuilder : MappingBuilder("GET", "/nhs/origin_appointment") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Zesty</title>
                    <body>
                        <h1>Zesty</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
