package features.sharedSteps

import io.cucumber.java.en.Given
import mocking.MockingClient




open class ExternalSiteMockingStepDefinitions {

    @Given("^'(.*)' responds to requests for '(.*)'$")
    fun responseToRequestsForBloodDonor(serviceProvider: String, serviceName: String) {
        when (serviceProvider) {
            "Blood Donor" ->
                MockingClient.instance.forExternalSites.mock {
                    bloodDonorRequest().respondWithPage()
                }
            "Informatica" ->
                MockingClient.instance.forExternalSites.mock {
                    informaticaRequest().respondWithPage()
                }
            "111" ->
                MockingClient.instance.forExternalSites.mock {
                    oneOneOneOnlineRequest(serviceName).respondWithPage(serviceName)
                }
            "NHS UK" ->
                MockingClient.instance.forExternalSites.mock {
                    nhsUkRequest(serviceName).respondWithPage(serviceName)
                }
            "Organ Donation" ->
                MockingClient.instance.forExternalSites.mock {
                    organDonationRequest(serviceName).respondWithPage(serviceName)
                }
        }
    }
}
