package mocking.models

import com.google.gson.Gson
import mocking.MockingClient
import java.time.Duration

class Mapping() {

    var request: Request? = null
    var response: Response? = null
    var scenarioName: String? = null
    var requiredScenarioState: String? = null
    var newScenarioState: String? = null

    constructor(request: Request, response: Response) : this() {
        this.request = request
        this.response = response
    }

    fun delayedBy(seconds: Duration): Mapping {
        this.response!!.fixedDelayMilliseconds = seconds.toMillis().toInt()
        return this
    }

    fun inScenario(title: String?): Mapping {
        if (title != null) this.scenarioName = title
        return this
    }

    fun whenScenarioStateIs(state: String?): Mapping {
        if (state != null) this.requiredScenarioState = state
        return this
    }

    fun willSetStateTo(state: String?): Mapping {
        if (state != null) this.newScenarioState = state
        return this
    }


    override fun toString(): String {
        val requestString = "Request: ${Gson().toJson(request)}"
        val responseString = "Response: ${Gson().toJson(response)}"
        val scenarioName = if (scenarioName != null) "scenarioName: ${Gson().toJson(scenarioName)}," else ""
        val requiredScenarioState = if (requiredScenarioState != null) "requiredScenarioState: ${Gson().toJson(
                requiredScenarioState)}," else ""
        val newScenarioState =
                if (newScenarioState != null) "newScenarioState: ${Gson().toJson(newScenarioState)}," else ""
        return "Mapping: $scenarioName $requiredScenarioState $newScenarioState $requestString, $responseString"

    }
}