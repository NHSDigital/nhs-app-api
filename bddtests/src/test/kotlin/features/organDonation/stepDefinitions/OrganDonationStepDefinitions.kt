package features.organDonation.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationChoicePage
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
        factory.existingOptOut()
    }

    @Given("I am a (\\w+) user registered with organ donation to donate all organs")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existingOptIn()
    }

    @Given("I am a (\\w+) user registered with organ donation with an appointed representative")
    fun iAmRegisteredWithOrganDonationWithAnAppointedRepresentative(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existingAppointedRepresentative()
    }

    @Given("I am a (\\w+) user registered with organ donation to donate some organs")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existingOptInSome()
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register$")
    fun iAmNotRegisteredWithOrganDonationWishToRegister(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithNotFoundError() }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt out$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithNotFoundError() }
        factory.optOut { registration -> registration.respondWithSuccess("test") }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt in$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithNotFoundError() }
        factory.optIn { registration -> registration.respondWithSuccess("test") }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and donate some organs$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndDonateSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithNotFoundError() }
        factory.some { registration -> registration.respondWithSuccess("test") }
    }

    @Given("I am a (\\w+) user not registered with organ donation, who wishes to opt out but will cause a conflict")
    fun iAmAUserNotRegisteredWithOrganDonationButRegistrationWillCauseConflict(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithNotFoundError() }
        factory.optOut { registration -> registration.respondWithConflict("test") }
    }

    @Given("I am a (\\w+) user registered with organ donation but existing registration is in conflicted state")
    fun iAmAUserRegisteredWithOrganDonationButExistingRegistrationIsInConflictedState(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithConflictError() }
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
        header.waitForNativeStepToComplete()
        header.waitForPageHeaderText("Organ donation register")
        navbarSteps.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }
}
