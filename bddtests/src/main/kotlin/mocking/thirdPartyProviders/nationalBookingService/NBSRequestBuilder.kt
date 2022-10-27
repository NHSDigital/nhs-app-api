package mocking.thirdPartyProviders.nationalBookingService
import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class NBSRequestBuilder(queryStringEntries: HashMap<String, String> = hashMapOf())
    : MappingBuilder("GET", "/book-a-coronavirus-vaccination/start-page") {
    init {
        if (queryStringEntries.isNotEmpty()) {
            queryStringEntries.entries.forEach {
                requestBuilder.andQueryParameter(it.key, it.value)
            }
        }
    }

    fun appointmentBookingsRequest() = NBSRequestBuilder()

    fun respondWithPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andHtmlBody("""
                <html>
                    <title>National Booking Service</title>
                    <body>
                        <h1>National Booking Service</h1>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}
