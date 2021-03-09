package mocking.thirdPartyProviders.gncr

import mocking.models.Mapping
import org.apache.http.HttpStatus

class GNCRAppointmentsRequestBuilder
    : GNCRRequestBuilder("GET", "/appointment") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>GNCR</title>
                    <body>
                        <h1>GNCR</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
