package mocking

import mocking.models.Mapping
import org.apache.http.HttpStatus

private const val SC_FOUND = HttpStatus.SC_MOVED_TEMPORARILY
const val CONTENT_TYPE_APPLICATION_JSON = "application/json; charset=UTF-8"

// See https://wiremock.org/docs/request-matching/#url-matching
enum class WiremockUrlMatch {
    Url,
    UrlPath,
    UrlPattern,
}

abstract class MappingBuilder(
        method: String,
        var urlPathOrPattern: String,
        var urlMatchType: WiremockUrlMatch = WiremockUrlMatch.UrlPath
) {

    open var delayMillisecs = 0
    internal val requestBuilder = RequestBuilder(method, urlPathOrPattern)

    fun respondWithBody(body: String, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body)
        }
    }

    fun respondWith(
            statusCode: Int,
            milliSecondDelay: Int = delayMillisecs,
            resolve: ResponseBuilder.() -> Unit
    ): Mapping {
        val responseBuilder = ResponseBuilder(statusCode)
        responseBuilder.resolve()

        if (milliSecondDelay > 0) responseBuilder.andDelay(milliSecondDelay)

        val request = when (urlMatchType) {
            WiremockUrlMatch.Url -> requestBuilder.buildForUrl()
            WiremockUrlMatch.UrlPath -> requestBuilder.buildForUrlPath()
            WiremockUrlMatch.UrlPattern -> requestBuilder.buildForUrlPattern(urlPathOrPattern)
        }

        return Mapping(request, responseBuilder.build())
    }

    fun respondWithSuccessJson(jsonBody: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(jsonBody, GsonFactory.asPascal)
        }
    }

    fun redirectTo(url: String, milliSecondDelay: Int = 0): Mapping {
        val responseBuilder = ResponseBuilder(SC_FOUND)
        responseBuilder.andHeader("Location", url).andTemplateTransformer()

        if (milliSecondDelay > 0) responseBuilder.andDelay(milliSecondDelay)

        return Mapping(requestBuilder.buildForUrlPath(), responseBuilder.build())
    }

    open fun respondWithServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("")
        }
    }

    open fun respondWithBadGateway(): Mapping {
        return respondWith(HttpStatus.SC_BAD_GATEWAY) {
            andXmlBody("")
        }
    }

    open fun respondWithCorruptedContent(content: String? = null): Mapping {
        return respondWith(HttpStatus.SC_OK) { andJsonBody(content ?: "{blah}") }
    }
}
