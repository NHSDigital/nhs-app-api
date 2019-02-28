package mocking.spine.ePS.prescriptions

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.spine.ePS.EPSMappingBuilder
import mocking.spine.ePS.models.SpineItemDetailGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

private const val ISSUE_NUMBER = "issueNumber"
private const val PRESCRIPTION_ID = "prescriptionId"

class EPS111ItemDetailBuilder (prescriptionId: String,
                               issueNumber: String?
)
    : EPSMappingBuilder( "GET", "/nhs111itemdetails" ) {

    init {
        requestBuilder.andQueryParameter(PRESCRIPTION_ID, prescriptionId)

        if (issueNumber != null) {
            requestBuilder.andQueryParameter(ISSUE_NUMBER, issueNumber)
        }
    }

    fun respondWithSuccess(spineItemDetailGetResponse: SpineItemDetailGetResponse): Mapping {
        return respondWithSuccessAny(spineItemDetailGetResponse)
    }


    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonBuilder().setFieldNamingPolicy(FieldNamingPolicy.IDENTITY).create())
        }
    }
}