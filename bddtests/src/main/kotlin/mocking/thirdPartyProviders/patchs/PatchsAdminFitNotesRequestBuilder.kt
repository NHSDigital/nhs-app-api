package mocking.thirdPartyProviders.patchs

import mocking.models.Mapping
import org.apache.http.HttpStatus

class PatchsAdminFitNotesRequestBuilder
    : PatchsRequestBuilder("GET", "/nhs-app-auth/admin-hub") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>PATCHS</title>
                    <body>
                        <h1>PATCHS</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
