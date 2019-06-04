package features.nominatedPharmacy.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.nominatedPharmacy.steps.NominatedPharmacyDataSetupSteps
import net.thucydides.core.annotations.Steps
import pages.assertElementNotPresent
import pages.prescription.PrescriptionsPage

class NominatedPharmacyErrorScenarioStepDefinitions {

    private lateinit var prescriptionsPage: PrescriptionsPage

    @Steps
    private lateinit var nominatedPharmacyDataSetupSteps: NominatedPharmacyDataSetupSteps

    @Given("^the request to PDS Trace to retrieve my nominated pharmacy fails$")
    fun requestToSpineToGetNominatedPharmacyFails() {
        nominatedPharmacyDataSetupSteps.setupWiremockForNominatedPharmacyWhenSpineFails()
    }

    @Given("^the request to Azure search to retrieve my nominated pharmacy fails$")
    fun requestToNHSSearchToGetNominatedPharmacyDetailsFails() {
        nominatedPharmacyDataSetupSteps.setupWiremockForNominatedPharmacyWhenAzureSearchFails()
    }

    @Then("^I do not see the nominated pharmacy panel$")
    fun iDoNotSeeTheNominatedPharmacyPanel() {
        prescriptionsPage.nominatedPharmacyPanel.assertElementNotPresent()
    }
}
