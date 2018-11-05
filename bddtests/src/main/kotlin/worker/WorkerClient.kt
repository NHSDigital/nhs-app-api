package worker

import com.google.gson.FieldNamingPolicy
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import config.Config
import org.apache.http.client.HttpClient
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase
import org.apache.http.client.protocol.HttpClientContext
import org.apache.http.impl.client.BasicCookieStore
import org.apache.http.impl.client.HttpClients
import org.apache.http.impl.cookie.BasicClientCookie
import org.apache.http.protocol.BasicHttpContext
import org.apache.http.protocol.HttpContext
import java.net.URI

class WorkerClient(config:Config = Config.instance) {
    private val _client: HttpClient
    private val gsonBuilder: GsonBuilder = GsonBuilder()
    private var gson: Gson
    private var workerClientSender: WorkerClientSender

    init {
        _client = HttpClients.createDefault()
        gsonBuilder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY)
        gson = gsonBuilder.create()
        workerClientSender = WorkerClientSender()
    }

    val appointments = WorkerClientAppointments(config, workerClientSender, gson)
    val prescriptions = WorkerClientPrescriptions(config, workerClientSender, gson)
    val myRecord = WorkerClientMyRecord(config, workerClientSender, gson)
    val authentication = WorkerClientAuthentication(config, workerClientSender, gson)

    companion object {

        fun getHttpContext(includeBadCookie: Boolean): HttpContext {
            val localContext = BasicHttpContext()
            val cookieStore = BasicCookieStore()

            if (includeBadCookie) {
                val cookie = BasicClientCookie("NHSO-Session-Id", "ErrorCookieId")
                cookie.path = "/"
                cookie.domain = ".bitraft.io"
                cookie.setAttribute("httponly", "null")
                cookie.setAttribute("path", "/")
                cookie.setAttribute("samesite", "lax")
                cookie.isSecure = false
                cookie.version = 0

                cookieStore.addCookie(cookie)
            }

            localContext.setAttribute(HttpClientContext.COOKIE_STORE, cookieStore)

            return localContext
        }
    }

    class HttpDeleteWithBody constructor() : HttpEntityEnclosingRequestBase() {
        companion object {
            const val METHOD_NAME = "DELETE"
        }

        constructor(uri: String) : this() {
            setURI(URI.create(uri))
        }

        override fun getMethod(): String {
            return METHOD_NAME
        }
    }
}