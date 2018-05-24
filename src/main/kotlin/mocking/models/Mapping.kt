package mocking.models

import com.google.gson.Gson
import mocking.MockingClient
import java.time.Duration

class Mapping() {

    var request: Request? = null
    var response: Response? = null


    constructor(request: Request, response: Response) : this() {
        this.request = request
        this.response = response
    }

    fun delayedBy(seconds: Duration): Mapping {
        this.response!!.fixedDelayMilliseconds = seconds.toMillis().toInt()
        return this
    }

    override fun toString(): String {
        val requestString = "Request: ${Gson().toJson(request)}"
        val responseString = "Response: ${Gson().toJson(response)}"
        return "Mapping: $requestString, $responseString"
    }
}