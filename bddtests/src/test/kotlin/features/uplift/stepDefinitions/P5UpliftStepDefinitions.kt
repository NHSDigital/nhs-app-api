package features.uplift.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject
import pages.P5UpliftPage
import utils.SerenityHelpers

class P5UpliftStepDefinitions : HybridPageObject() {

  val mockingClient = MockingClient.instance

  @Steps
  private lateinit var p5ShutterPage: P5UpliftPage

  @Given("^I am patient using the (.*) with proof level 5 access$")
  fun userWithProofLevelFive(gpSystem: String) {
    val supplier = Supplier.valueOf(gpSystem)
    mockingClient.clearWiremock()
    mockingClient.favicon()

    val patient = Patient.getDefault(supplier)
    patient.identityProofingLevel = IdentityProofingLevel.P5

    SerenityHelpers.setPatient(patient)
    SerenityHelpers.setGpSupplier(supplier)

    CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
  }

  @Then("I am told to finish setting up my NHS login")
  fun assertP5ShutterPageContent() {
    p5ShutterPage.assertUpliftBanner()
  }
}
