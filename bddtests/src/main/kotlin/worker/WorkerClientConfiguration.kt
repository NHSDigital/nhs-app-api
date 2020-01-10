package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.methods.HttpGet
import worker.models.configuration.ConfigurationResponse
import worker.models.configuration.ConfigurationV2Response

class WorkerClientConfiguration(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getConfiguration(nativeAppVersion: String, deviceName: String): ConfigurationResponse {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.configuration +
                "?nativeappversion=$nativeAppVersion&devicename=$deviceName")

        val response = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson(response, ConfigurationResponse::class.java)
    }

    fun getConfigurationv2(): ConfigurationV2Response {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.configurationV2)

        val response = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson(response, ConfigurationV2Response::class.java)
    }
}
