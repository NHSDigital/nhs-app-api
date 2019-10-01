package features.authentication.factories

import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.Error

class Im1ConnectionV2GetFactoryTpp : Im1ConnectionV2GetFactory("TPP") {

    override fun errorIm1Verify(httpStatusCode: Int, errorCode: String,
                                message: String?) {
        authenticate(httpStatusCode, errorCode, message)
        val error = Error(
                errorCode,
                "Mocked TPP Error"
        )
        mockingClient.forTpp {
            authentication.linkAccountRequest(patient).respondWithError(error, httpStatusCode)
        }
    }

    private fun authenticate(httpStatusCode: Int, errorCode: String, message: String?) {

        val authenticateRequest = Authenticate(
                apiVersion = "1",
                accountId = patient.accountId,
                passphrase = patient.passphrase,
                unitId = "A82648",
                uuid = "af0a8175-e6c2-4c49-883e-020b2b3600f9",
                application = Application(
                        name = "NhsApp",
                        version = "1.0",
                        providerId = "b891fc3b51d5e7c1",
                        deviceType = "NhsApp"
                )
        )

        mockingClient.forTpp {
            authentication.authenticateRequest(authenticateRequest)
                    .respondWithError(httpStatusCode, errorCode, message)
        }

    }
}
