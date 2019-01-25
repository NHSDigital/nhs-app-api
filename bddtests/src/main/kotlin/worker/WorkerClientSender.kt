package worker

import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.apache.http.client.HttpClient
import org.apache.http.client.config.RequestConfig
import org.apache.http.client.methods.HttpUriRequest
import org.apache.http.impl.client.HttpClientBuilder
import org.apache.http.impl.client.HttpClients
import org.apache.http.protocol.HttpContext
import utils.SerenityHelpers
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.SocketTimeoutException

class WorkerClientSender{

    private var _client: HttpClient
    private var csrfToken = ""
    private var connectionTimeoutSeconds: Int? = null

    init {
        _client = HttpClients.createDefault()
    }

    fun setCsrfToken(token: String) {
        this.csrfToken = token
    }

    fun setConnectionTimeout(timeout: Int) {
        this.connectionTimeoutSeconds = timeout
    }

    fun sendAsyncAndGetResult(request: HttpUriRequest, context: HttpContext? = null): String? {
        val response = sendAsync(request, context)
        if(response != null) {
            val rd = BufferedReader(InputStreamReader(response.entity.content))
            val result = rd.use { it.readText() }
            return result
        }else {
            return null
        }
    }

    fun sendAsync(request: HttpUriRequest, context: HttpContext? = null): HttpResponse? {
        // If we have a token, use it
        if (this.csrfToken.isNotEmpty()) {
            request.addHeader("X-CSRF-TOKEN", csrfToken)
        }

        if(connectionTimeoutSeconds != null) {
            val requestConfigBuilder = RequestConfig.custom()
                    .setConnectionRequestTimeout(connectionTimeoutSeconds!!)
                    .setConnectTimeout(connectionTimeoutSeconds!!)
                    .setSocketTimeout(connectionTimeoutSeconds!!)
                    .build()

            _client = HttpClientBuilder
                    .create()
                    .setDefaultRequestConfig(requestConfigBuilder)
                    .build()
        }

        var response: HttpResponse

        try {
            response = if (context != null) _client.execute(request, context) else _client.execute(request)
        } catch(ex: SocketTimeoutException) {
            return null
        }
        SerenityHelpers.setHttpResponse(response)

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