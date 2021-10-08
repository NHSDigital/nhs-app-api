package features.uplift.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.MockingClient
import mocking.citizenId.login.UpliftLoginRequestBuilder
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject
import pages.P5UpliftPage
import pages.navigation.WebHeader
import pages.withNormalisedText
import utils.SerenityHelpers
import pages.assertIsVisible

class P5UpliftStepDefinitions : HybridPageObject() {

  private val mockingClient = MockingClient.instance

  @Steps
  private lateinit var p5ShutterPage: P5UpliftPage

  lateinit var webHeader: WebHeader

  @Given("^I am a patient with proof level 5$")
  fun iAmAPatientWithProofLevel5() {
    setupPatient()
  }

  @Given("^I am a patient with proof level 5 who wishes to view the Health A to Z$")
  fun iAmAUserWhoWishesToViewAdviceAboutCoronavirus() {
    setupPatient()
    MockingClient.instance.forExternalSites.mock { healthAToZRequest().respondWithPage() }
  }

  @Given("^I am a patient with proof level 5 who wishes to view 111 online$")
  fun iAmAUserWhoWishesToViewOneOneOneOnline() {
    setupPatient()
    MockingClient.instance.forExternalSites.mock {
      oneOneOneOnlineRequest("/home").respondWithPage("/home") }
  }

  private fun setupPatient() {
    val supplier = Supplier.EMIS
    mockingClient.favicon()

    val patient = Patient.getDefault(supplier).copy(identityProofingLevel = IdentityProofingLevel.P5)

    SerenityHelpers.setPatient(patient)
    SerenityHelpers.setGpSupplier(supplier)

    CitizenIdSessionCreateJourney().createFor(patient)
    SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
  }

  @Then("^I am asked to prove my identity$")
  fun iAmAskedToProveMyIdentity() {
    p5ShutterPage.assertUpliftBanner()
  }

  @Then("^I am asked to prove my identity to access '(.*)'$")
  fun iAmAskedToProveMyIdentityToAccessPage(page: String) {
    p5ShutterPage.assertUpliftBanner(page)
  }

  @Then("^the uplift journey starts$")
  fun theUpliftJourneyStarts() {
    webHeader.getPageTitle().withNormalisedText(UpliftLoginRequestBuilder.title).assertIsVisible()
  }
}
