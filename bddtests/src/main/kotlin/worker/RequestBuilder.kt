package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPatch
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.methods.HttpPut
import org.apache.http.client.methods.HttpRequestBase
import org.apache.http.protocol.HttpContext
import javax.servlet.http.Cookie

open class RequestBuilder protected constructor(private var request: HttpRequestBase) {

    companion object {
        fun post(path: String): RequestBuilderWithBody {
            return RequestBuilderWithBody(HttpPost(path))
        }

        fun get(path: String): RequestBuilder {
            return RequestBuilder(HttpGet(path))
        }

        fun patch(path: String): RequestBuilderWithBody {
            return RequestBuilderWithBody(HttpPatch(path))
        }

        fun delete(path: String): RequestBuilderWithBody {
            return RequestBuilderWithBody(HttpDeleteWithBody(path))
        }

        fun put(path: String): RequestBuilderWithBody {
            return RequestBuilderWithBody(HttpPut(path))

        }
    }

    fun addExternalSystemApiKey(includeApiKey: Boolean): RequestBuilder {
        if (includeApiKey) {
            val key = Config.instance.nhsAppApiKey
            request.addHeader("X-Api-Key", key)
        }
        return this
    }

    fun addAuthorizationIfNotNull(authToken: String?): RequestBuilder {
        if (authToken != null) {
            request.addHeader("Authorization", "Bearer $authToken")
        }
        return this
    }

    fun addCookieIfNotNull(sessionCookie: Cookie?): RequestBuilder {
        if (sessionCookie != null) {
            request.addHeader("Cookie", sessionCookie.value.split(";")[0])
        }
        return this
    }

    fun setHeader(key: String, value: String?): RequestBuilder {
        request.setHeader(key, value)
        return this
    }

    fun addHeader(key: String, value: String): RequestBuilder {
        request.addHeader(key, value)
        return this
    }

    fun send(sender: WorkerClientSender): HttpResponse? {
        val response = sender.sendAsync(request)
        request.releaseConnection()
        return response
    }

    fun <T> sendAndGetResult(sender: WorkerClientSender,
                             gson: Gson,
                             classOfT: Class<T>,
                             context: HttpContext? =null): T? {
        val response = sendAndGetResult(sender, context)
        return if (response == null) null else gson.fromJson(response, classOfT)
    }

    fun sendAndGetResult(sender: WorkerClientSender, context: HttpContext? =null): String? {
        val response = sender.sendAsyncAndGetResult(request, context)
        request.releaseConnection()
        return response
    }
}
