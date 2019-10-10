package features.authentication.stepDefinitions

import config.Config
import cucumber.api.DataTable
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.CIDAccountCreationSteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ServiceUnavailablePage
import pages.account.MyAccountPage
import pages.navigation.BreadcrumbHeader
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import utils.SerenityHelpers

const val INVALID_VALUE = "xxx-wrong-format-xxx"

class AuthenticationStepDefinitions {

    @Steps
    lateinit var accountCreation: CIDAccountCreationSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps
    @Steps
    lateinit var webHeader: WebHeader
    lateinit var breadcrumbs: BreadcrumbHeader
    lateinit var myAccount: MyAccountPage
    lateinit var serviceUnavailablePage: ServiceUnavailablePage
    lateinit var currentUrl: String

    val mockingClient = MockingClient.instance

    @Given("^I have just logged out$")
    fun iHaveJustLoggedOut() {
        browser.goToApp()
        login.using(EmisMockDefaults.patientEmis)
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
    }


    @Given("^I am logged in as a (.*) user where the session will fail to clear on signout$")
    fun iAmLoggedInToWhereSessionFailsToClear(gpSystem: String) {
        if (gpSystem.toUpperCase() != "TPP") {
            Assert.fail("'$gpSystem' not set up for this step")
        }
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        //Whereas the usual TppSessionCreateJourneyFactory.createFor includes the logOff request,
        //createAuthenticateRequest does not.
        TppSessionCreateJourneyFactory(mockingClient).createAuthenticateRequest(patient)
        mockingClient.forTpp { authentication.logOffRequest().respondWithError() }

        browser.goToApp()
        login.using(patient)
    }


    @Given("^I want to register for a (.+) account$")
    fun iWantToRegisterForAnAccount(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)

        browser.goToApp()
    }

    @When("I log out")
    fun iLogOut() {
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
    }

    @When("I use the header link to log out of the website")
    fun iLogOutUsingHeaderLink() {
        webHeader.clickLogout()
    }

    @When("I log in again")
    fun iLogInAgain() {
        val patient = SerenityHelpers.getPatient()
        login.using(patient)
        home.waitForLoginToCompleteSuccessfully()
    }

    @When("^I browse to the page at (.*)$")
    fun iBrowseToPageAt(url: String) {
        this.currentUrl = nav.browseToPage(url)
    }

    @When("^I browse to the pages at the following urls I see the (.*) page$")
    fun iBrowseToPageAtXAndSeeTheXPage(page: String, table: DataTable) {
        val lowerPage = page.toLowerCase()
        for (row in table.raw()) {
            val fullUrl = Config.instance.url + row.get(0)
            browser.browseTo(fullUrl)
            this.currentUrl = fullUrl

            when (lowerPage) {
                "login" -> {
                    login.loginPage.shouldBeDisplayed()
                }
                "home" -> {
                    home.assertHeaderVisible()
                }
                "relevant" -> {
                    browser.shouldHaveUrl(Config.instance.url + row.get(1))
                }
            }
        }
    }

    @When("^I select to create an account$")
    fun iClickCreateAccountButton() {
        login.loginPage.loginOrCreateAccountButton.click()
    }

    @When("^I have completed (.+) account creation$")
    fun iCreateAnAccount(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        iWantToRegisterForAnAccount(gpSystem)
        login.loginPage.createAccount(patient)
    }

    @When("^I sign out")
    fun iClickTheSignOutButton() {
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
        browser.waitUntilSignoutCompletes()
    }


    @Then("I can cycle through the header links")
    fun iLCycleTheHeaderLinks() {
        val linksToFollow = arrayListOf(
                { NativeHeaderHelper.followSymptomsHeaderLink(webHeader) },
                { NativeHeaderHelper.followAppointmentHeaderLink(webHeader) },
                { NativeHeaderHelper.followPrescriptionsHeaderLink(webHeader) },
                { NativeHeaderHelper.followMedicalRecordHeaderLink(webHeader) },
                { NativeHeaderHelper.followAccountHeaderLink(webHeader) }
        )

        linksToFollow.forEachIndexed { index, link ->
            if (index != linksToFollow.size)
                link.invoke()
        }
    }

    @Then("I can cycle through the native header links")
    fun iLCycleTheNativeHeaderLinks(){
        val linksToFollow = arrayListOf(
                {followAppointmentNativeHeaderLink()},
                {followPrescriptionsNativeHeaderLink()},
                {followMyRecordNativeHeaderLink()},
                {followSymptomsNativeHeaderLink()}
        )

        linksToFollow.forEachIndexed { index, link ->
            if (index != linksToFollow.size)
                link.invoke()
        }
    }

    private fun followAppointmentNativeHeaderLink() {
        nav.select(NavBarNative.NavBarType.APPOINTMENTS)
        webHeader.isPageTitleCorrect("Appointments")
        breadcrumbs.assertVisible()
    }

    private fun followPrescriptionsNativeHeaderLink() {
        nav.select(NavBarNative.NavBarType.PRESCRIPTIONS)
        webHeader.isPageTitleCorrect("Repeat prescriptions")
        breadcrumbs.assertVisible()
    }

    private fun followMyRecordNativeHeaderLink() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
        webHeader.isPageTitleCorrect("My medical record")
        breadcrumbs.assertVisible()
    }

    private fun followSymptomsNativeHeaderLink() {
        nav.select(NavBarNative.NavBarType.SYMPTOMS)
        webHeader.isPageTitleCorrect("Symptoms")
        breadcrumbs.assertVisible()
    }

    @Then("^I see an error message informing me I cannot log in as I am under the minimum age$")
    fun iSeeAnErrorMessageInformingMeICannotLogInAsIAmUnderSixteen() {
        serviceUnavailablePage.assertIsPresent("You are too young to use the NHS App",
                "Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. " +
                        "You can still call or visit your GP surgery to access your NHS services. " +
                        "For urgent medical advice, visit 111.nhs.uk or call 111.")
    }

    @Then("^I see the login page$")
    fun iSeeTheLoginPage() {
        login.loginPage.shouldBeDisplayed()
    }

    @Then("^I see the navigation menu$")
    fun iSeeNavbar() {
        nav.assertVisible()
    }

    @Then("^I do not see the yellow banner$")
    fun iDoNotSeeYellowBanner() {
        home.homePage.assertYellowBannerIsNotPresent()
    }

    @Then("^I do not see the menu bar$")
    fun iDoNotSeeMenuBar() {
        login.loginPage.assertMenuIsNotVisible()
    }

    @Then("^the user login details are cleared from cookies$")
    @Throws(Exception::class)
    fun theUserLoginDetailsAreClearedFromCookies() {
        browser.checkLoginDetailsAreReset()
    }

    @Then("^I see the CID create an account page$")
    fun thenISeeTheCIDCreateAnAccountPage() {
        accountCreation.assertPageIsVisible()
    }

    @Then("^I see the signed in home page$")
    fun thenISeeTheSignedInHomePage() {
        navHeader.assertHomePageHeaderVisible()
    }
}
