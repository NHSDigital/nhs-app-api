package mocking.ndop

import mocking.models.Mapping
import org.apache.http.HttpStatus

class NdopLinkRequestBuilder()
    : NdopMappingBuilder("GET") {

    init {
        requestBuilder
    }

    fun respondWithNdopMockPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andTemplatedHtmlBody("""
                <html>
                    <body>
                        <form>
                            <label name="ndop_ok">Linked to Ndop OK</label>
                        </form>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}