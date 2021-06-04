package features.silverIntegration.nhsd.stepDefinitions

import io.cucumber.java.en.Given
import mocking.MockingClient
import mocking.thirdPartyProviders.nhsd.NhsdRequestBuilder
import pages.HybridPageObject

class NhsdCompanyStepDefinitions : HybridPageObject() {
    @Given("^NHSD responds to requests to check your Covid vaccine record$")
    fun nhsdCompanyRespondsToRequestsToCheckYourCovidVaccineRecord() {
        MockingClient.instance.forNhsd.mock {
            NhsdRequestBuilder().vaccineRecordRequest().respondWithPage()
        }
    }
}
