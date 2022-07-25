package mocking.thirdPartyProviders.netcall

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class NetcallRequestBuilder : MappingBuilder("GET", "/Appointments") {

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>Netcall</title>
                    <body>
                        <h1>Netcall</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
