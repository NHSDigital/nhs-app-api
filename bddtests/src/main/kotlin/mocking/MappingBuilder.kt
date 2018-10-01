package mocking

import mocking.models.Mapping
import org.apache.http.HttpStatus

private const val SC_FOUND = HttpStatus.SC_MOVED_TEMPORARILY
const val CONTENT_TYPE_APPLICATION_JSON = "application/json; charset=UTF-8"
abstract class MappingBuilder(method: String, url: String) {

    internal val requestBuilder = RequestBuilder(method, url)

    fun respondWithBody(body: String, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body)
        }
    }

    fun respondWith(statusCode: Int, milliSecondDelay: Int = 0, resolve: ResponseBuilder.() -> Unit): Mapping {
        val responseBuilder = ResponseBuilder(statusCode)
        responseBuilder.resolve()

        if (milliSecondDelay > 0) responseBuilder.andDelay(milliSecondDelay)

        return Mapping(requestBuilder.build(), responseBuilder.build())
    }

    fun respondWithSuccessJson(jsonBody: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(jsonBody, GsonFactory.asPascal)
        }
    }

    fun respondWithFailureJson(jsonBody: String, statusCode: Int): Mapping {
        return respondWith(statusCode){
            andBody(jsonBody, contentType = "application/json")
        }
    }

    fun redirectTo(url: String, milliSecondDelay: Int = 0): Mapping {
        val responseBuilder = ResponseBuilder(SC_FOUND)
        responseBuilder.andHeader("Location", url).andTemplateTransformer()

        if (milliSecondDelay > 0) responseBuilder.andDelay(milliSecondDelay)

        return Mapping(requestBuilder.build(), responseBuilder.build())
    }
}
