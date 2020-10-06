package features.healthAdvice.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.HealthAdvicePage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.externalSites.AdviceAboutCoronavirusPage
import pages.externalSites.HealthAToZPage
import pages.externalSites.OneOneOneOnlinePage
import utils.SerenityHelpers

open class HealthAdviceStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    private lateinit var healthAdvicePage: HealthAdvicePage
    private lateinit var adviceAboutCoronavirusPage: AdviceAboutCoronavirusPage
    private lateinit var healthAToZPage: HealthAToZPage
    private lateinit var oneOneOneOnlinePage: OneOneOneOnlinePage

    @Given("^I am a user who wishes to view advice about coronavirus$")
    fun iAmAUserWhoWishesToViewAdviceAboutCoronavirus() {
        setupUser()
        MockingClient.instance.favicon()
        MockingClient.instance.forExternalSites.mock { adviceAboutCoronavirusRequest().respondWithPage() }
    }

    @Given("^I am a user who wishes to view the Health A to Z$")
    fun iAmAUserWhoWishesToViewTheHealthAToZ() {
        setupUser()
        MockingClient.instance.favicon()
        MockingClient.instance.forExternalSites.mock { healthAToZRequest().respondWithPage() }
    }

    @Given("^I am a user who wishes to view 111 online$")
    fun iAmAUserWhoWishesToViewOneOneOneOnline() {
        setupUser()
        MockingClient.instance.favicon()
        MockingClient.instance.forExternalSites.mock { oneOneOneOnlineRequest().respondWithPage() }
    }

    private fun setupUser() {
        val supplier = Supplier.valueOf("EMIS")
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)

        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @When("^I click Search Conditions and Treatments$")
    fun clickSearchConditionsAndTreatments() {
        browser.storeCurrentTabCount()
        healthAdvicePage.searchConditionsAndTreatments.click()
    }

    @When("^I click Use NHS 111 online$")
    fun clickUserNhsOneOneOneOnline() {
        browser.storeCurrentTabCount()
        healthAdvicePage.useNhsOneOneOneOnline.click()
    }

    @When("^I click Advice About Coronavirus$")
    fun clickAdviceAboutCoronavirus() {
        browser.storeCurrentTabCount()
        healthAdvicePage.adviceAboutCoronavirus.click()
    }

    @When("^I click Ask your GP for Advice$")
    fun clickAskYourGpForAdvice() {
        healthAdvicePage.askYourGpForAdvice.click()
    }

    @Then("^the Advice page is displayed")
    fun getHealthAdvicePageIsDisplayed() {
        healthAdvicePage.searchConditionsAndTreatments.assertIsVisible()
        healthAdvicePage.useNhsOneOneOneOnline.assertIsVisible()
        healthAdvicePage.adviceAboutCoronavirus.assertIsVisible()
    }

    @Then("^the advice about coronavirus page has been opened in a new tab$")
    fun theAdviceAboutCoronavirusPageHasBeenOpenedInANewTab() {
        browser.changeTab(adviceAboutCoronavirusPage.url)
        adviceAboutCoronavirusPage.assertTitleVisible()
    }

    @Then("^the health A to Z page has been opened in a new tab$")
    fun theHealthAToZPageHasBeenOpenedInANewTab() {
        browser.changeTab(healthAToZPage.url)
        healthAToZPage.assertTitleVisible()
    }

    @Then("^the 111 online page has been opened in a new tab$")
    fun theOneOneOneOnlinePageHasBeenOpenedInANewTab() {
        browser.changeTab(oneOneOneOnlinePage.url)
        oneOneOneOnlinePage.assertTitleVisible()
    }

    @Then("^the link to Engage Medical Advice is not available on the Advice page$")
    fun theLinkToEngageMedicalAdviceIsNotAvailableOnTheAdvicePage() {
        healthAdvicePage.engageMedicalAdvice.assertElementNotPresent()
    }
}
