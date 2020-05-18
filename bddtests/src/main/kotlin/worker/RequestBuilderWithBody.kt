package worker

import com.google.gson.Gson
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase
import org.apache.http.entity.StringEntity

class RequestBuilderWithBody(private var request: HttpEntityEnclosingRequestBase) : RequestBuilder(request) {

    fun <T> addBody(body: T, gson: Gson): RequestBuilder {
        val jsonRequest = gson.toJson(body)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        request.entity = entity
        return this
    }
}