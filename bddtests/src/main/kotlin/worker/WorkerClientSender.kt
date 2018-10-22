package worker

import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.apache.http.client.HttpClient
import org.apache.http.client.methods.HttpUriRequest
import org.apache.http.impl.client.HttpClients
import org.apache.http.protocol.HttpContext
import java.io.BufferedReader
import java.io.InputStreamReader

class WorkerClientSender{

    private val _client: HttpClient
    private var csrfToken = ""

    init {
        _client = HttpClients.createDefault()
    }

    fun setCsrfToken(token: String) {
        this.csrfToken = token
    }

    fun sendAsyncAndGetResult(request: HttpUriRequest, context: HttpContext? = null): String {
        val response = sendAsync(request, context)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        return result
    }

    fun sendAsync(request: HttpUriRequest, context: HttpContext? = null): HttpResponse {
        // If we have a token, use it
        if (this.csrfToken.isNotEmpty()) {
            request.addHeader("X-CSRF-TOKEN", csrfToken)
        }

        val response = if (context != null) _client.execute(request, context) else _client.execute(request)

        if (response.statusLine.statusCode != HttpStatus.SC_OK &&
                response.statusLine.statusCode != HttpStatus.SC_CREATED &&
                response.statusLine.statusCode != HttpStatus.SC_NO_CONTENT) {
            // Exception is thrown here to ensure that the
            // tests fail at the appropriate location and not further down the line
            // when values are not as expected.  This makes it easier to debug.
            throw NhsoHttpException(request, response)
        } else {
            return response
        }
    }
}