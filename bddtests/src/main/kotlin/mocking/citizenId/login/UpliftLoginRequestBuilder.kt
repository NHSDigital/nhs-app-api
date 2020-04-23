package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class UpliftLoginRequestBuilder(
        val patient: Patient,
        redirectUri: String,
        clientId: String,
        matcherToUse: String? = null)
    : CitizenIdMappingBuilder("GET", "/authorize") {

    init {
        val matcherString = matcherToUse ?: "equalTo"
        requestBuilder
                .andQueryParameter("redirect_uri", redirectUri, matcherString)
                .andQueryParameter("client_id", clientId)
                .andQueryParameter("response_type", "code")
                .andQueryParameter("vtr", "[\"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"]")
    }

    fun respondWithUpliftPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andTemplatedHtmlBody("""
            <html>
                <body>
                    <h1>${title}</h1>
                </body>
            </html>
        """.trimIndent())
        }
    }

    companion object {
        const val title :String = "Uplift journey"
    }
}
