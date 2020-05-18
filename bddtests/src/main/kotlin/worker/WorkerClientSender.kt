package worker

import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.apache.http.client.HttpClient
import org.apache.http.client.config.RequestConfig
import org.apache.http.client.methods.HttpUriRequest
import org.apache.http.impl.client.HttpClientBuilder
import org.apache.http.impl.client.HttpClients
import org.apache.http.protocol.HttpContext
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.set
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.SocketTimeoutException

class WorkerClientSender {

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
        if (response != null && response.entity != null) {
            val rd = BufferedReader(InputStreamReader(response.entity.content))
            val result = rd.use { it.readText() }
            return result
        } else {
            return null
        }
    }

    fun sendAsync(request: HttpUriRequest, context: HttpContext? = null): HttpResponse? {
        if (this.csrfToken.isNotEmpty()) {
            request.addHeader("X-CSRF-TOKEN", csrfToken)
        }

        if (connectionTimeoutSeconds != null) {
            setTimeoutOnClient()
        }

        val response = try {
            if (context != null) _client.execute(request, context) else _client.execute(request)
        } catch (ex: SocketTimeoutException) {
            return null
        }
        SerenityHelpers.setHttpResponse(response)
        val isSuccessful = isSuccessful(response)
        return if (!isSuccessful) {
            GlobalSerenityHelpers.HTTP_EXCEPTION.set(NhsoHttpException(request, response))
            null
        } else response
    }

    private fun isSuccessful(response: HttpResponse):Boolean {
        val successfulStatusCodes = arrayListOf(HttpStatus.SC_OK, HttpStatus.SC_CREATED, HttpStatus.SC_NO_CONTENT)
        return successfulStatusCodes.contains(response.statusLine.statusCode)
    }

    private fun setTimeoutOnClient() {
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
}
