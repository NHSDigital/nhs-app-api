package worker

import com.google.gson.FieldNamingPolicy
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import config.Config

import org.apache.http.HttpResponse
import org.apache.http.HttpStatus.SC_CREATED
import org.apache.http.HttpStatus.SC_OK
import org.apache.http.client.HttpClient
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.methods.HttpUriRequest
import org.apache.http.impl.client.HttpClients
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import org.apache.http.entity.StringEntity
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import worker.models.session.UserSessionResponse.UserSessionResponseCookie
import java.io.InputStreamReader
import java.io.BufferedReader
import javax.servlet.http.Cookie


class WorkerClient {

    var cookieHeaderKey = "Set-Cookie"
    private val _client: HttpClient
    private val gsonBuilder: GsonBuilder = GsonBuilder()
    private var gson: Gson
    private val config = Config.instance

    init {
        _client = HttpClients.createDefault()
        gsonBuilder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY)
        gson = gsonBuilder.create()
    }


    fun getIm1Connection(connectionToken: String?, odsCode: String?): Im1ConnectionResponse {
        val httpGet = HttpGet(config.backendUrl + WorkerPaths.patientIm1Connection)
        httpGet.setHeader(WorkerHeaders.ConnectionToken, connectionToken)
        httpGet.setHeader(WorkerHeaders.OdsCode, odsCode)

        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()

        return gson.fromJson(result, Im1ConnectionResponse::class.java)

    }

    fun postIm1Connection(requestBody: Im1ConnectionRequest): Im1ConnectionResponse {
        val httpPost = HttpPost(config.backendUrl + WorkerPaths.patientIm1Connection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpPost.releaseConnection()
        println(result)
        return gson.fromJson<Im1ConnectionResponse>(result, Im1ConnectionResponse::class.java)
    }

    fun postSessionConnection(requestBody: UserSessionRequest): UserSessionResponse {
        val httpPost = HttpPost(config.backendUrl + WorkerPaths.sessionConnection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpPost.releaseConnection()
        println(result)

        val userSessionResponseBody = gson.fromJson<UserSessionResponse.UserSessionResponseBody>(result,
                UserSessionResponse.UserSessionResponseBody::class.java)

        val userSessionResponseCookie = UserSessionResponseCookie(Cookie(cookieHeaderKey, response.getHeaders(cookieHeaderKey).first().value))
        return UserSessionResponse(userSessionResponseCookie, userSessionResponseBody)
    }

    private fun sendAsync(request: HttpUriRequest): HttpResponse {
        val response = _client.execute(request)
        if (response.statusLine.statusCode != SC_OK && response.statusLine.statusCode != SC_CREATED) {
            // Exception is thrown here to ensure that the tests fail at the appropriate location and not further down the line
            // when values are not as expected.  This makes it easier to debug.
            throw NhsoHttpException(request, response)
        } else {
            return response
        }
    }
}