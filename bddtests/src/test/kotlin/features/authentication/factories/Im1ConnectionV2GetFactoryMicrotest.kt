package features.authentication.factories
import constants.Supplier
import mocking.defaults.MicrotestMockDefaults
import org.apache.http.HttpStatus
import utils.SerenityHelpers

class Im1ConnectionV2GetFactoryMicrotest : Im1ConnectionV2GetFactory(Supplier.MICROTEST) {

    override fun errorIm1Verify(httpStatusCode: Int, errorCode: String,
                                message: String?) {
        val patient = MicrotestMockDefaults.patient
        SerenityHelpers.setPatient(patient)

        if (httpStatusCode == HttpStatus.SC_BAD_GATEWAY)
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithServiceUnavailable()
        }

        else if (httpStatusCode == HttpStatus.SC_FORBIDDEN || httpStatusCode == HttpStatus.SC_INTERNAL_SERVER_ERROR)
            mockingClient.forMicrotest.mock {
                demographics.demographicsRequest(patient).respondWithInternalServerError()
            }
        else {
            throw Exception ("Test setup incorrect, this status code $httpStatusCode has not yet been implemented")
        }
    }
}