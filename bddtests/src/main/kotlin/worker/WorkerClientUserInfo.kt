package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.utils.URIBuilder
import worker.models.userInfo.UserAndInfoResponse

class WorkerClientUserInfo(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun postUserInfo(authToken: String?): HttpResponse {
        val httpPost = HttpPost(config.apiBackendUrl + WorkerPaths.userMeInfo)

        if (authToken != null) {
            httpPost.addHeader("Authorization", "Bearer $authToken")
        }

        val response = sender.sendAsync(httpPost)
        httpPost.releaseConnection()
        return response!!
    }

    fun getUserInfo(authToken: String?): UserAndInfoResponse {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.userMeInfo)

        if (authToken != null) {
            httpGet.addHeader("Authorization", "Bearer $authToken")
        }

        val response = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson(response, UserAndInfoResponse::class.java)
    }

    fun getUserInfo(odsCode: String?, nhsNumber: String?, includeApiKey: Boolean): Array<String> {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userInfo)
        if (odsCode != null) {
            uriBuilder.setParameter("odsCode", odsCode)
        }
        if (nhsNumber != null) {
            uriBuilder.setParameter("nhsNumber", nhsNumber)
        }
        val path = uriBuilder.build()
        val httpGet = HttpGet(path)
        httpGet.addExternalSystemApiKey(includeApiKey)


        val response = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson(response, Array<String>::class.java)
    }
}