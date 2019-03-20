package worker

import org.apache.http.HttpRequest
import org.apache.http.HttpResponse
import java.io.BufferedReader
import java.io.InputStreamReader

class NhsoHttpException(
        val uri: String,
        val statusCode: Int,
        val body: String?,
        val method: String? = null) : RuntimeException() {

    constructor(request: HttpRequest, response: HttpResponse) : this(
            request.requestLine.uri,
            response.statusLine.statusCode,
            parseBody(response),
            request.requestLine.method
    )

    override fun toString(): String {
        val builder = StringBuilder()
        builder.append("HTTP exception:" + "\r\n")
        builder.append("Request URI: $uri\r\n")
        builder.append("Method:      $method\r\n")
        builder.append("Status code: $statusCode\r\n")
        builder.append("Body:        $body\r\n")
        return builder.toString()
    }

    companion object {

        private fun parseBody(response: HttpResponse): String? {
            val rd = BufferedReader(InputStreamReader(response.entity.content))
            return rd.use { it.readText() }
        }
    }
}
