package worker

import org.apache.http.HttpRequest
import org.apache.http.HttpResponse

class NhsoHttpException(request: HttpRequest, response: HttpResponse, var method: String? = null) : RuntimeException() {

    val Body: String = response.toString()
    val RequestUri: String = request.requestLine.uri
    var StatusCode: Int = response.statusLine.statusCode

    init {
        method = request.requestLine.method
    }

    override fun toString(): String {
        val builder = StringBuilder()
        builder.append("Stubs threw an HTTP exception:" + "\r\n")
        builder.append(String.format("  Status code: %1\$s", StatusCode) + "\r\n")
        builder.append(String.format("  Request URI: %1\$s", RequestUri) + "\r\n")
        builder.append(String.format("  Method:      %1\$s", method) + "\r\n")
        builder.append("  Body:" + "\r\n")
        builder.append(Body + "\r\n")
        return builder.toString()
    }
}