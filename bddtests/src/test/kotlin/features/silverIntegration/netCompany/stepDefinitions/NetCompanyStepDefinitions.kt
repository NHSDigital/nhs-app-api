package features.silverIntegration.netCompany.stepDefinitions

import io.cucumber.java.en.Given
import mocking.MockingClient
import mocking.thirdPartyProviders.netCompany.NetCompanyRequestBuilder
import pages.HybridPageObject

class NetCompanyStepDefinitions : HybridPageObject() {
    @Given("^Net Company responds to requests to share Covid Status$")
    fun netCompanyRespondsToRequestsToShareCovidStatus() {
        MockingClient.instance.forNetCompany.mock { 
            NetCompanyRequestBuilder().vaccineRecordRequest().respondWithPage() 
        }
    }
}
