package mocking

import com.google.gson.Gson
import config.Config
import mocking.models.Request

class RequestBuilder internal constructor(private val method: String, private val path: String) {

    private var headers: MutableMap<String, Map<String, String>> = mutableMapOf()
    private var queryParameters: MutableMap<String, Map<String, String>> = mutableMapOf()
    private var bodyPatterns: MutableList<Map<String, String>> = mutableListOf()

    fun andJsonBody(body: Any, condition: String = "equalToJson", gson: Gson = Gson()): RequestBuilder {
        if (condition.isNotEmpty()) {
            bodyPatterns.add(mapOf(Pair(condition, gson.toJson(body))))
        }

        if (!headers.containsKey("Content-Type")) {
            andHeader("Content-Type", "application/json; charset=UTF-8")
        }
        return this
    }

    fun andBody(body: String, condition: String = "equalToJson"): RequestBuilder {
        bodyPatterns.add(hashMapOf(condition to body))
        return this
    }

    fun andQueryParameter(name: String, value: String, condition: String = "equalTo"): RequestBuilder {
        queryParameters[name] = mapOf(Pair(condition, value))
        return this
    }

    fun andHeader(name: String, value: String, condition: String = "equalTo"): RequestBuilder {
        headers[name] = mapOf(Pair(condition, value))
        return this
    }

    internal fun build(): Request {
        return Request(path, method, headers, queryParameters, bodyPatterns)
    }
}