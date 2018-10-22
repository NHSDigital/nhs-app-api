package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.methods.HttpGet
import worker.models.demographics.Demographics
import worker.models.myrecord.MyRecordResponse

class WorkerClientMyRecord(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun getDemographics(): Demographics {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getDemographicsConnection)
        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson<Demographics>(result, Demographics::class.java)
    }

    fun getMyRecord(): MyRecordResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getMyRecordConnection)
        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, MyRecordResponse::class.java)
    }

}