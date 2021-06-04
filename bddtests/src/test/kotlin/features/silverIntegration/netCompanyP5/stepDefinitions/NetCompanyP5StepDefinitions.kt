package features.silverIntegration.netCompanyP5.stepDefinitions

import io.cucumber.java.en.Given
import mocking.MockingClient
import mocking.thirdPartyProviders.netCompany.NetCompanyRequestBuilder
import pages.HybridPageObject

class NetCompanyP5StepDefinitions : HybridPageObject() {
    @Given("^Net Company responds to requests to share Covid Status P5$")
    fun netCompanyRespondsToRequestsToShareCovidStatusP5() {
        MockingClient.instance.forNetCompany.mock { 
            NetCompanyRequestBuilder().vaccineRecordRequest().respondWithPage() 
        }
    }
}
