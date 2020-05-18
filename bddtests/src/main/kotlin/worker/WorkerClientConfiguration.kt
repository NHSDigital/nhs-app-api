package worker

import com.google.gson.Gson
import config.Config
import worker.models.configuration.ConfigurationResponse
import worker.models.configuration.ConfigurationV2Response

class WorkerClientConfiguration(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getConfiguration(nativeAppVersion: String, deviceName: String): ConfigurationResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.configuration +
                "?nativeappversion=$nativeAppVersion&devicename=$deviceName")
        return httpGet.sendAndGetResult(sender, gson, ConfigurationResponse::class.java)
    }

    fun getConfigurationv2(): ConfigurationV2Response? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.configurationV2)
        return httpGet.sendAndGetResult(sender, gson, ConfigurationV2Response::class.java)
    }
}
