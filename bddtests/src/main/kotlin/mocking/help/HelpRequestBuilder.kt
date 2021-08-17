package mocking.help

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class HelpRequestBuilder : MappingBuilder("GET", "/help/(.*)", true) {
    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andTemplatedHtmlBody("""
                <html>
                    <title>Help page</title>
                    <body>
                        <h1>{{request.path}}</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
