package features.healthAdvice.stepDefinitions

import constants.Supplier
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journeys.termsAndConditions.TermsAndConditionsJourneyFactory
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

    @Given("^I am a user with coronavirus information disabled$")
    fun iAmAUser() {
        setupUser(SJRJourneyType.CORONAVIRUS_INFORMATION_DISABLED)
    }

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

    private fun setupUser(journeyType: SJRJourneyType = SJRJourneyType.CORONAVIRUS_INFORMATION_ENABLED) {
        val supplier = Supplier.valueOf("EMIS")

        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
            supplier,
            journeyType
        )

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

    private fun getHealthAdvicePageIsDisplayed() {
        healthAdvicePage.searchConditionsAndTreatments.assertIsVisible()
        healthAdvicePage.useNhsOneOneOneOnline.assertIsVisible()
    }

    @Then("^the Advice page is displayed")
    fun getHealthAdvicePageWithCoronavirusAdviceDisplayed() {
        getHealthAdvicePageIsDisplayed()
        healthAdvicePage.adviceAboutCoronavirus.assertIsVisible()
    }

    @Then("^the Advice page is displayed without advice about coronavirus")
    fun getHealthAdvicePageWithoutCoronavirusAdviceDisplayed() {
        getHealthAdvicePageIsDisplayed()
        healthAdvicePage.adviceAboutCoronavirus.assertElementNotPresent()
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
