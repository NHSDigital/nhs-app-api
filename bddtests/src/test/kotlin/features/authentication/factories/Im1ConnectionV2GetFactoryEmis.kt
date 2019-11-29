package features.authentication.factories

import constants.Supplier

class Im1ConnectionV2GetFactoryEmis : Im1ConnectionV2GetFactory(Supplier.EMIS) {

    override fun errorIm1Verify(httpStatusCode: Int, errorCode: String,
                                message: String?) {
        mockingClient.forEmis {
            authentication.endUserSessionRequest().respondWithError(httpStatusCode, errorCode, message)
        }
    }
}