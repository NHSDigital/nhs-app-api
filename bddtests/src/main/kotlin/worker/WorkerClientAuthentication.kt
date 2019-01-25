package worker

import com.google.gson.Gson
import com.google.gson.JsonParser
import config.Config
import mockingFacade.linkage.LinkageInformationFacade
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse
import worker.models.ndop.NdopResponse
import worker.models.patient.Im1ConnectionResponse
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.io.BufferedReader
import java.io.InputStreamReader
import javax.servlet.http.Cookie

class WorkerClientAuthentication(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    var cookieHeaderKey = "Set-Cookie"

    fun getIm1Connection(connectionToken: String?, odsCode: String?): Im1ConnectionResponse {
        val httpGet = HttpGet(config.cidBackendUrl + WorkerPaths.patientIm1Connection)
        httpGet.setHeader(WorkerHeaders.ConnectionToken, connectionToken)
        httpGet.setHeader(WorkerHeaders.OdsCode, odsCode)

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, Im1ConnectionResponse::class.java)
    }

    fun postSessionConnection(requestBody: UserSessionRequest): UserSessionResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.sessionConnection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsync(httpPost)
        val rd = BufferedReader(InputStreamReader(response!!.entity.content))
        val result = rd.use { it.readText() }
        httpPost.releaseConnection()
        println(result)

        // Extract csrfToken
        val parsedResponse = JsonParser().parse(result)
        sender.setCsrfToken(parsedResponse.asJsonObject.get("token").asString)

        val userSessionResponseBody = gson.fromJson<UserSessionResponse.UserSessionResponseBody>(result,
                UserSessionResponse.UserSessionResponseBody::class.java)

        val userSessionResponseCookie = UserSessionResponse.UserSessionResponseCookie(Cookie(
                cookieHeaderKey, response.getHeaders(cookieHeaderKey).first().value))
        return UserSessionResponse(userSessionResponseCookie, userSessionResponseBody)
    }

    fun getNdopToken(): NdopResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.ndopConnection)
        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson<NdopResponse>(result, NdopResponse::class.java)
    }

    fun getLinkageKey(linkage: LinkageInformationFacade): LinkageResponse {
        val httpGet = HttpGet(config.cidBackendUrl + WorkerPaths.LinkageKey)
        httpGet.setHeader(WorkerHeaders.NhsNumber, linkage.nhsNumber)
        httpGet.setHeader(WorkerHeaders.OdsCode,linkage. odsCode)
        httpGet.setHeader(WorkerHeaders.IdentityToken,linkage. identityToken)
        httpGet.setHeader(WorkerHeaders.Surname, linkage.surname)
        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        println(result)

        return gson.fromJson(result, LinkageResponse::class.java)
    }

    fun postLinkageKey(requestBody: CreateLinkageRequest, timeout: Int? = null): LinkageResponse {
        val httpPost = HttpPost(config.cidBackendUrl + WorkerPaths.LinkageKey)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        if(timeout != null) {
            sender.setConnectionTimeout(timeout)
        }
        val result = sender.sendAsyncAndGetResult(httpPost)
        httpPost.releaseConnection()
        println(result)
        if(result !=  null) {
            return gson.fromJson<LinkageResponse>(result, LinkageResponse::class.java)
        } else {
            return LinkageResponse("default", "default", "default")
        }
    }
}