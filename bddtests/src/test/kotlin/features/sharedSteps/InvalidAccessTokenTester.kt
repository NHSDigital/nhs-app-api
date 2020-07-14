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

        private fun assertInvalidTokenThrowsUnauthorised(
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
                    HttpStatus.SC_UNAUTHORIZED,
                    errorResponse!!.statusCode)
        }
    }
}
