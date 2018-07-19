package worker

import org.apache.http.HttpRequest
import org.apache.http.HttpResponse

class NhsoHttpException(
        val uri: String,
        val statusCode: Int,
        val body: String?,
        val method: String? = null) : RuntimeException() {

    constructor(request: HttpRequest, response: HttpResponse) : this(
            request.requestLine.uri,
            response.statusLine.statusCode,
            response.toString(),
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
}