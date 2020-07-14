package mocking

import com.google.gson.Gson
import mocking.models.Response

class ResponseBuilder(private val statusCode: Int) {
    companion object {
        private val gson = Gson()
    }

    private var body: String? = null
    private var transformers: MutableList<String> = mutableListOf()
    private var fixedDelayMilliseconds: Int? = null
    private var headers: MutableMap<String, String> = mutableMapOf()

    fun andJsonBody(body: Any, gsonOverride: Gson? = null): ResponseBuilder {
        val json = (gsonOverride ?: gson).toJson(body)
        return andBody(json, "application/json")
    }

    fun andJsonBody(jsonBody: String):ResponseBuilder{
        return andBody(jsonBody, "application/json")
    }

    fun andTemplatedHtmlBody(body: String): ResponseBuilder {
        return andTemplateTransformer()
                .andHtmlBody(body)
    }

    fun andHtmlBody(body: String): ResponseBuilder {
        return andBody(body, "text/html")
    }

    fun andXmlBody(body: String): ResponseBuilder {
        return andBody(body, "text/xml")
    }

    fun andBody(body: String, contentType: String): ResponseBuilder {
        this.body = body
        andHeader("Content-Type", contentType)
        return this
    }

    fun andTemplateTransformer(): ResponseBuilder {
        transformers.add("response-template")
        return this
    }

    fun andHeader(name: String, value: String): ResponseBuilder {
        headers[name] = value
        return this
    }

    fun andDelay(delayMilliseconds: Int): ResponseBuilder {
        fixedDelayMilliseconds = delayMilliseconds
        return this
    }

    fun build() = Response(statusCode, body, transformers, fixedDelayMilliseconds, headers)
}
