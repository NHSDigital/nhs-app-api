package mocking.thirdPartyProviders.substrakt

import mocking.models.Mapping
import org.apache.http.HttpStatus

class SubstraktParticipationRequestBuilder
    : SubstraktRequestBuilder("GET", "/join-ppg") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Substrakt</title>
                    <body>
                        <h1>Substrakt</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
