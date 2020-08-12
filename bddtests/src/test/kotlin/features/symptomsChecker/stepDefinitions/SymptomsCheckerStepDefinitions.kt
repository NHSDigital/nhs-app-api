package features.symptomsChecker.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.CheckMySymptomsPage
import pages.assertIsVisible
import pages.externalSites.AdviceAboutCoronavirusPage
import pages.externalSites.HealthAToZPage
import pages.externalSites.OneOneOneOnlinePage
import pages.navigation.NavBarNative
import utils.SerenityHelpers

open class SymptomsCheckerStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var navBar: NavigationSteps

    private lateinit var checkMySymptomsPage: CheckMySymptomsPage
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
        checkMySymptomsPage.searchConditionsAndTreatments.click()
    }

    @When("^I click Use NHS 111 online$")
    fun clickUserNhsOneOneOneOnline() {
        browser.storeCurrentTabCount()
        checkMySymptomsPage.useNhsOneOneOneOnline.click()
    }

    @When("^I click Advice About Coronavirus$")
    fun clickAdviceAboutCoronavirus() {
        browser.storeCurrentTabCount()
        checkMySymptomsPage.adviceAboutCoronavirus.click()
    }

    @When("^I click Ask your GP for Advice$")
    fun clickAskYourGpForAdvice() {
        checkMySymptomsPage.askYourGpForAdvice.click()
    }

    @Then("^the Symptoms page is displayed")
    fun checkMySymptomsPageIsDisplayed() {
        checkMySymptomsPage.searchConditionsAndTreatments.assertIsVisible()
        checkMySymptomsPage.useNhsOneOneOneOnline.assertIsVisible()
        checkMySymptomsPage.adviceAboutCoronavirus.assertIsVisible()
    }

    @Then("^the Symptoms page header and navigation menu are correct$")
    fun checkMySymptomsPageHeaderAndNavigationMenuAreCorrect() {
        navBar.headerNative.assertIsVisible("Symptoms")
        navBar.assertSelectedTab(NavBarNative.NavBarType.SYMPTOMS)
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
}
