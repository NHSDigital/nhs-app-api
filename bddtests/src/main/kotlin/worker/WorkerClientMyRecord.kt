package worker

import com.google.gson.Gson
import config.Config
import worker.models.demographics.Demographics
import worker.models.myrecord.MyRecordResponse

class WorkerClientMyRecord(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getDemographics(patientId: String): Demographics? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.getDemographicsConnection)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, Demographics::class.java)
    }

    fun getMyRecord(patientId: String?): MyRecordResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.getMyRecordConnection)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, MyRecordResponse::class.java)
    }
}
