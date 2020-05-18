package worker

import com.google.gson.Gson
import com.google.gson.JsonParser
import config.Config
import mockingFacade.linkage.LinkageInformationFacade
import worker.models.authorization.RefreshAccessTokenResponse
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse
import worker.models.linkedAccounts.LinkedAccountsConfigResponse
import worker.models.ndop.NdopResponse
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.URI
import javax.servlet.http.Cookie

class WorkerClientAuthentication(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    var cookieHeaderKey = "Set-Cookie"

    fun postIm1Connection(im1ConnectionRequest: Im1ConnectionRequest): Im1ConnectionResponse? {
        val httpPost = RequestBuilder.post(config.cidBackendUrl + WorkerPaths.patientIm1ConnectionV1)
                .addBody(im1ConnectionRequest, gson)
        return httpPost.sendAndGetResult(sender, gson, Im1ConnectionResponse::class.java)
    }

    fun getIm1Connection(connectionToken: String?, odsCode: String?): Im1ConnectionResponse? {
        val httpGet = RequestBuilder.get(config.cidBackendUrl + WorkerPaths.patientIm1ConnectionV1)
                .setHeader(WorkerHeaders.ConnectionToken, connectionToken)
                .setHeader(WorkerHeaders.OdsCode, odsCode)
        return httpGet.sendAndGetResult(sender, gson, Im1ConnectionResponse::class.java)
    }

    fun getIm1ConnectionV2(connectionToken: String?, odsCode: String?): Im1ConnectionResponse? {
        val httpGet = RequestBuilder.get(config.cidBackendUrl + WorkerPaths.patientIm1ConnectionV2)
                .setHeader(WorkerHeaders.ConnectionToken, connectionToken)
                .setHeader(WorkerHeaders.OdsCode, odsCode)
        return httpGet.sendAndGetResult(sender, gson, Im1ConnectionResponse::class.java)
    }

    fun postIm1ConnectionV2(im1ConnectionRequest: Im1ConnectionRequest): Im1ConnectionResponse? {
        val uri = URI(Config.instance.cidBackendUrl + WorkerPaths.patientIm1ConnectionV2)
        val httpPost = RequestBuilder.post(uri.toString())
                .addBody(im1ConnectionRequest, gson)
        return httpPost.sendAndGetResult(sender, gson, Im1ConnectionResponse::class.java)
    }

    fun postSessionConnection(requestBody: UserSessionRequest): UserSessionResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.sessionConnection)
                .addBody(requestBody, gson)

        val response = httpPost.send(sender)
        if(response!=null) {
            val rd = BufferedReader(InputStreamReader(response.entity.content))
            val result = rd.use { it.readText() }

            // Extract csrfToken
            val parsedResponse = JsonParser().parse(result)
            sender.setCsrfToken(parsedResponse.asJsonObject.get("token").asString)

            val userSessionResponseBody = gson.fromJson<UserSessionResponse.UserSessionResponseBody>(result,
                    UserSessionResponse.UserSessionResponseBody::class.java)

            val userSessionResponseCookie = UserSessionResponse.UserSessionResponseCookie(Cookie(
                    cookieHeaderKey, response.getHeaders(cookieHeaderKey)?.first()?.value))
            return UserSessionResponse(userSessionResponseCookie, userSessionResponseBody)
        }
        return null
    }

    fun getNdopToken(patientId: String): NdopResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.ndopConnection)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, NdopResponse::class.java)
    }

    fun getLinkageKey(linkage: LinkageInformationFacade): LinkageResponse? {
        val httpGet = RequestBuilder.get(config.cidBackendUrl + WorkerPaths.LinkageKey)
                .setHeader(WorkerHeaders.NhsNumber, linkage.nhsNumber)
                .setHeader(WorkerHeaders.OdsCode, linkage.odsCode)
                .setHeader(WorkerHeaders.IdentityToken, linkage.identityToken)
                .setHeader(WorkerHeaders.Surname, linkage.surname)
        return httpGet.sendAndGetResult(sender, gson, LinkageResponse::class.java)
    }

    fun postLinkageKey(requestBody: CreateLinkageRequest, timeout: Int? = null): LinkageResponse {
        if (timeout != null) {
            sender.setConnectionTimeout(timeout)
        }

        val httpPost = RequestBuilder.post(config.cidBackendUrl + WorkerPaths.LinkageKey)
                .addBody(requestBody, gson)

        val result = httpPost.sendAndGetResult(sender)
        println(result)
        if (result != null) {
            return gson.fromJson<LinkageResponse>(result, LinkageResponse::class.java)
        } else {
            return LinkageResponse("default", "default", "default")
        }
    }

    fun getPatientLinkedAccountsConfiguration(): LinkedAccountsConfigResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.patientConfiguration)
        return httpGet.sendAndGetResult(sender, gson, LinkedAccountsConfigResponse::class.java)
    }

    fun postRefreshAccessToken(sessionCookie: Cookie? = null): RefreshAccessTokenResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.refreshAccessToken)
                .addCookieIfNotNull(sessionCookie)
        return httpPost.sendAndGetResult(sender, gson, RefreshAccessTokenResponse::class.java)
    }
}



