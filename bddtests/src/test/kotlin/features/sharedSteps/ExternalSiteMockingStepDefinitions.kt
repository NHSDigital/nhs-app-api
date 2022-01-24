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

    @Given("^'(.*)' responds to requests for type '(.*)'$")
    fun responseToRequestsForLink(serviceProvider: String, serviceName: String) {
        when (serviceProvider) {
            "NHS COVID Pass" ->
                MockingClient.instance.forExternalSites.mock {
                    getCovidPassRequest().respondWithPage()
                }
            "COVID Pass or proof" ->
                MockingClient.instance.forExternalSites.mock {
                    getCovidPassOrProofRequest(serviceName).respondWithPage(serviceName)
                }
            "Northern Ireland" ->
                MockingClient.instance.forExternalSites.mock {
                    getNorthernIrelandRequest().respondWithPage()
                }
            "My Health Online" ->
                MockingClient.instance.forExternalSites.mock {
                    oneOneOneOnlineRequest(serviceName).respondWithPage(serviceName)
                }
        }
    }
}
