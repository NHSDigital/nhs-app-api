package mocking.ndop

import mocking.models.Mapping
import org.apache.http.HttpStatus

class NdopLinkRequestBuilder
    : NdopMappingBuilder("POST") {

    fun respondWithNdopMockPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andTemplatedHtmlBody("""
                <html>
                    <body>
                        <p>{{request.body}}</p>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}