package features.organDonation.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationNewRegistrationPage
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.organdonation.OrganDonationSearchResponse
import java.net.URL

private const val NEW_TAB_WAIT_TIME = 1000L

open class OrganDonationStepDefinitions {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var browser: BrowserSteps
    lateinit var header: HeaderNative

    lateinit var organDonationNewRegistrationPage: OrganDonationNewRegistrationPage

    protected val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^I am a (.*) user not registered with organ donation, who wishes to register$")
    fun iAmNotRegisteredWithOrganDonationWishToRegister(gpSystem: String) {
        OrganDonationFactory().unregisteredUser(gpSystem)
        val patient = SerenityHelpers.getPatient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
    }

    @Given("^I am a (.*) user registered with organ donation$")
    fun iAmAlreadyRegisteredWithOrganDonation(gpSystem: String) {
        OrganDonationFactory().registeredUser(gpSystem)
    }

    @Given("^I am a (.*) user not registered with organ donation$")
    fun iAmNotRegisteredWithOrganDonation(gpSystem: String) {
        OrganDonationFactory().unregisteredUser(gpSystem)
    }

    @Given("^I am a (.*) user registered with organ donation, but organ donation will conflict$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillConflict(gpSystem: String) {
        OrganDonationFactory().conflict(gpSystem)

    }

    @Given("^I am a (.*) user registered with organ donation, but organ donation call will time out$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillThrowTimeout(gpSystem: String) {
        OrganDonationFactory().organDonationTimeout(gpSystem)
    }

    @Given("^I am a (.*) user registered with organ donation, but demographics will time out$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowTimeOutError(gpSystem: String) {
        OrganDonationFactory().demographicsTimeout(gpSystem)
    }

    @Given("^the organ donation toggle is set to target the internal page$")
    fun toggleOrganDonationSiteLink() {
        browser.appendSourceQueryString("ios")
    }

    @When("^I request my organ donation details$")
    fun iRequestMyOrganDonationDetails() {
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .getOrganDonationConnection()
            setSessionVariable(OrganDonationSearchResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive organ donation details$")
    fun iReceiveOrganDonationDetails() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertNotEquals("Organ donation decision incorrect",
                "NotFound",
                organDonationResponse.decision)
        Assert.assertNotNull("Organ donation identifier was not found", organDonationResponse.identifier)
    }

    @Then("^I receive no organ donation details$")
    fun iDoNotReceiveOrganDonationDetail() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertEquals("Organ donation decision incorrect",
                "NotFound",
                organDonationResponse.decision)
        Assert.assertNull("Organ donation identifier should be null",
                organDonationResponse.identifier)
    }

    @Then("^I receive the users demographics details$")
    fun iReceiveTheGpUsersDemographicsDetails() {
        val patient = SerenityHelpers.getPatient()

        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertEquals("Nhs number in response does not match the patient",
                patient.formattedNHSNumber(),
                organDonationResponse.nhsNumber)
        Assert.assertEquals("Patient name in the response does not match patient",
                patient.formattedFullName(),
                organDonationResponse.nameFull)
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
        Assert.assertTrue("Sub Title is not visible", organDonationNewRegistrationPage.isSubTitleVisible())
    }

    @Then("^the Organ Donation page is displayed$")
    fun theOrganDonationPageIsDisplayed() {
        header.waitForPageHeaderText("Register your organ donation decision ")
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
