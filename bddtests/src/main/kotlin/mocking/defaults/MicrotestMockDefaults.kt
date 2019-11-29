package mocking.defaults

import constants.Supplier
import models.Patient

class MicrotestMockDefaults{

    companion object {

        const val DEFAULT_ODS_CODE_MICROTEST: String = "A21410"
        const val DEFAULT_NHS_NUMBER: String = "5785445875"
        const val DEFAULT_CONNECTION_TOKEN: String = "{\"Im1CacheKey\" : \" test\", \"NhsNumber\" : \"" +
                DEFAULT_NHS_NUMBER + "\"}";
        val patient = Patient.getDefault(Supplier.MICROTEST)
    }
}
