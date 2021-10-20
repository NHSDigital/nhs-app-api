package features.sharedSteps

import mocking.AccessTokenBuilder
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.SerenityHelpers

class InvalidAccessTokenTester {

    companion object {
        fun assertInvalidTokensThrowUnauthorised(apiCall: (String) -> Unit) {
            val invalidTokens = AccessTokenBuilder().getInvalidTokens(SerenityHelpers.getPatient())
            invalidTokens.forEach { invalidToken ->
                assertInvalidTokenThrowsUnauthorised(
                        accessToken = invalidToken.first.serialize(),
                        tokenParameterKey = invalidToken.second,
                        apiCall = apiCall)
            }
        }

        fun assertInvalidTokensThrowInternalServer(apiCall: (String) -> Unit) {
            val invalidTokens = AccessTokenBuilder().getInvalidTokens(SerenityHelpers.getPatient())
            invalidTokens.forEach { invalidToken ->
                assertInvalidTokenThrowsInternalServer(
                        accessToken = invalidToken.first.serialize(),
                        tokenParameterKey = invalidToken.second,
                        apiCall = apiCall)
            }
        }

        private fun assertInvalidTokenThrowsUnauthorised(
                accessToken: String,
                tokenParameterKey: String,
                apiCall: (String) -> Unit) {
            doCallAndThrow(
                    HttpStatus.SC_UNAUTHORIZED,
                    accessToken,
                    tokenParameterKey,
                    apiCall)
        }

        private fun assertInvalidTokenThrowsInternalServer(
                accessToken: String,
                tokenParameterKey: String,
                apiCall: (String) -> Unit) {
            doCallAndThrow(
                    HttpStatus.SC_INTERNAL_SERVER_ERROR,
                    accessToken,
                    tokenParameterKey,
                    apiCall)
        }

        private fun doCallAndThrow(
                error: Int,
                accessToken: String,
                tokenParameterKey: String,
                apiCall: (String) -> Unit) {
            SerenityHelpers.clearHttpException()
            apiCall.invoke(accessToken)
            val errorResponse = SerenityHelpers.getHttpException()
            Assert.assertNotNull(
                    "An exception was expected but was not returned within the expected time limit. " +
                            "Access Token invalid value: '$tokenParameterKey",
                    errorResponse
            )
            Assert.assertEquals("Incorrect status code returned. " +
                    "Access Token invalid value: '$tokenParameterKey",
                    error,
                    errorResponse!!.statusCode)
        }
    }
}
