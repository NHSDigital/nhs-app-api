package mocking.externalSites.nhsUK

import mocking.externalSites.ExternalSitesMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

open class NhsUkRequestBuilder(method: String ="GET", relativePath:String="")
    : ExternalSitesMappingBuilder(method, "/nhsuk$relativePath") {

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
