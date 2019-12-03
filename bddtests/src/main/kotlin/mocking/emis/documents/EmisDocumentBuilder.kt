package mocking.emis.documents

import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisDocumentBuilder(configuration: EmisConfiguration,
                          linkToken: String,
                          apiEndUserSessionId: String,
                          apiSessionId: String,
                          documentId: String)
    : EmisMappingBuilder(configuration, "GET", "/documents/$documentId") {

    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(documentResponse: DocumentResponseModel): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(documentResponse)
                    .build()
        }
    }
}