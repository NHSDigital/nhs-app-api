package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
import worker.models.userInfo.UserAndInfoResponse
import worker.models.userInfo.UserResearchPreferenceRequest

class WorkerClientUserInfo(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun postUserInfo(authToken: String?): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.userMeInfo)
                .addAuthorizationIfNotNull(authToken)

        return httpPost.send(sender)
    }

    fun postUserResearchPreference(authToken: String?, preference: UserResearchPreferenceRequest): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.userMeUserResearch)
                .addBody(preference, gson)
                .addAuthorizationIfNotNull(authToken)

        return httpPost.send(sender)
    }

    fun getUserInfo(authToken: String?): UserAndInfoResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.userMeInfo)
                .addAuthorizationIfNotNull(authToken)

        return httpGet.sendAndGetResult(sender, gson, UserAndInfoResponse::class.java)
    }

    fun getUserInfo(odsCode: String?, nhsNumber: String?, includeApiKey: Boolean): Array<String>? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userInfo)
                .setParameterIfNotNull("odsCode", odsCode)
                .setParameterIfNotNull("nhsNumber", nhsNumber)

        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
                .addExternalSystemApiKey(includeApiKey)

        return httpGet.sendAndGetResult(sender, gson, Array<String>::class.java)
    }
}