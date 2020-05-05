package features.uplift.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.citizenId.login.UpliftLoginRequestBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject
import pages.P5UpliftPage
import pages.assertIsVisible
import pages.navigation.WebHeader
import pages.withNormalisedText
import utils.SerenityHelpers
import pages.navigation.NavBarNative
import features.sharedSteps.NavigationSteps

class P5UpliftStepDefinitions : HybridPageObject() {

  val mockingClient = MockingClient.instance

  @Steps
  private lateinit var p5ShutterPage: P5UpliftPage
  @Steps
  lateinit var nav: NavigationSteps

  lateinit var webHeader: WebHeader

  @Given("^I am a patient with proof level 5$")
  fun iAmAPatientWithProofLevel5() {
    val supplier = Supplier.valueOf("EMIS")
    mockingClient.clearWiremock()
    mockingClient.favicon()

    val patient = Patient.getDefault(supplier).copy(identityProofingLevel = IdentityProofingLevel.P5)

    SerenityHelpers.setPatient(patient)
    SerenityHelpers.setGpSupplier(supplier)

    CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
  }

  @Then("^I am asked to prove my identity$")
  fun iAmAskedToProveMyIdentity() {
    p5ShutterPage.assertUpliftBanner()
  }

  @Then("^I am asked to prove my identity to access '(.*)'$")
  fun iAmAskedToProveMyIdentityToAccessPage(page: String) {
    p5ShutterPage.assertUpliftBanner(page)
  }

  @Then("the uplift journey starts")
  fun theUpliftJourneyStarts() {
    webHeader.getPageTitle().withNormalisedText(UpliftLoginRequestBuilder.title).assertIsVisible()
  }


  @Then("the navbar is working")
  fun checkTheNavbarIsWorking(){
    val linksToFollow = arrayListOf(
            {followAppointmentNativeNavBarLink()},
            {followPrescriptionsNativeNavBarLink()},
            {followMyRecordNativeNavBarLink()},
            {followSymptomsNativeNavBarLink()}
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

  private fun followMyRecordNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.MY_RECORD)
  }

  private fun followSymptomsNativeNavBarLink() {
    nav.select(NavBarNative.NavBarType.SYMPTOMS)
  }

}
