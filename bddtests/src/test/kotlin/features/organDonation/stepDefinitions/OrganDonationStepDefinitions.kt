package features.organDonation.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.organDonation.models.OrganDonationDemographics
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationChoicePage
import pages.organDonation.OrganDonationFaithModule
import java.net.URL

private const val NEW_TAB_WAIT_TIME = 1000L

open class OrganDonationStepDefinitions {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var browser: BrowserSteps

    lateinit var header: HeaderNative

    lateinit var organDonationChoicePage: OrganDonationChoicePage

    @Given("I am a (\\w+) user registered with organ donation to not donate my organs")
    fun iAmRegisteredWithOrganDonationToNotDonateOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.optOut()
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate all organs$")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.optIn()
    }

    @Given("I am a (\\w+) user registered with organ donation with an appointed representative")
    fun iAmRegisteredWithOrganDonationWithAnAppointedRepresentative(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.appointedRepresentative()
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate some organs$")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.optInSome()
    }

    @Given("I am a (\\w+) user registered with organ donation to donate some organs, but not all are decided on")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgansButNotAllDecidedOn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.optInSomeNotAllDecided()
    }

    @Given("^I am a (\\w+) user registered with organ donation with a decision to (.*) who wishes to withdraw$")
    fun iAmRegisteredWithOrganDonationAndWishToWithdraw(gpSystem: String, decision: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existing = factory.existing.setUpExistingDecisionForPatient(decision)
        factory.withdrawRegistration { request ->
            request.respondWithSuccess(existing.id)
        }
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate all organs with a faith decision of '(.*)'$")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgansAndDecisionOfFaith(gpSystem: String, faith: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = OrganDonationFaithModule.getFaith(faith))
        factory.existing.optIn(demographics)
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register$")
    fun iAmNotRegisteredWithOrganDonationWishToRegister(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(HttpStatus.SC_NOT_FOUND) }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt out$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration -> registration.optOut { request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt in$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration -> registration.optIn { request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to opt in with '(.*)' faith " +
            "decision$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptInWithFaithDecision(gpSystem: String, faith:String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        val demographics = OrganDonationDemographics(faithDeclaration = OrganDonationFaithModule.getFaith(faith))
        factory.create { registration -> registration.optIn(demographics) {
            request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and donate some organs$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndDonateSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration ->
            registration.some(OrganDonationRegistrationDataBuilder.someOrgansListUpdated())
            { request -> request.respondWithSuccess("test") }
        }
    }

    @Given("^the organ donation toggle is set to target the internal page$")
    fun toggleOrganDonationSiteLink() {
        organDonationChoicePage.waitForSpinnerToDisappear()
        browser.appendSourceQueryString("ios")
    }

    @Then("^the external Organ Donation page is displayed$")
    fun iAmOnTheExternalOrganDonationPage() {
        if (header.onMobile()) {
            pageOpensWithinNativeApp()
        } else {
            aNewTabOpens(Config.instance.organDonation)
        }
    }

    fun iAmOnTheOrganDonationPage() {
        iAmOnTheExternalOrganDonationPage()
    }

    @Then("^the internal Organ Donation page is displayed$")
    fun iAmOnTheInternalOrganDonationPage() {
        organDonationChoicePage.assertDisplayed()
    }

    private fun aNewTabOpens(url: String) {
        Thread.sleep(NEW_TAB_WAIT_TIME)
        browser.changeTab(URL(url))
        browser.shouldHaveUrl(url)
    }

    private fun pageOpensWithinNativeApp() {
        header.locatorMethods.waitForNativeStepToComplete()
        header.waitForPageHeaderText("Organ donation register")
        navbarSteps.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

}
