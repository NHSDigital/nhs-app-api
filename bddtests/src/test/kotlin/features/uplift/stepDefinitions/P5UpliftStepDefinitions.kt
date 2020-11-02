package features.uplift.stepDefinitions

import constants.Supplier
import features.sharedSteps.BrowserSteps
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.MockingClient
import mocking.citizenId.login.UpliftLoginRequestBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject
import pages.P5UpliftPage
import pages.navigation.WebHeader
import pages.withNormalisedText
import utils.SerenityHelpers
import pages.navigation.NavBarNative
import features.sharedSteps.NavigationSteps
import pages.assertIsVisible
import utils.GlobalSerenityHelpers
import utils.set

class P5UpliftStepDefinitions : HybridPageObject() {

  private val mockingClient = MockingClient.instance

  @Steps
  private lateinit var p5ShutterPage: P5UpliftPage
  @Steps
  lateinit var nav: NavigationSteps
  @Steps
  private lateinit var browser: BrowserSteps

  lateinit var webHeader: WebHeader

  @Given("^I am a patient with proof level 5$")
  fun iAmAPatientWithProofLevel5() {
    setupPatient()
  }

  @Given("^I am a patient logging in natively with proof level 5$")
  fun iAmAPatientLoggingInNativelyWithProofLevel5() {
    browser.setUserAgentSource("ios")
    GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
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
    MockingClient.instance.forExternalSites.mock { oneOneOneOnlineRequest().respondWithPage() }
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


  @Then("^the navbar is working$")
  fun checkTheNavbarIsWorking(){
    val linksToFollow = arrayListOf(
            {followAppointmentNativeNavBarLink()},
            {followPrescriptionsNativeNavBarLink()},
            {followYourHealthNativeNavBarLink()},
            {followAdviceNativeNavBarLink()}
    )

    linksToFollow.forEachIndexed { index, link ->
      if (index != linksToFollow.size)
        link.invoke()
    }
  }

  private fun followAppointmentNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.APPOINTMENTS)
  }

  private fun followPrescriptionsNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.PRESCRIPTIONS)
  }

  private fun followYourHealthNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.YOUR_HEALTH)
  }

  private fun followAdviceNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.ADVICE)
  }

}
