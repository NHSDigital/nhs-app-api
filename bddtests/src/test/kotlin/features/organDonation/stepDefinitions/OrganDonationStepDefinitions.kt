package features.organDonation.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.organDonation.models.OrganDonationDemographics
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus
import pages.HomePage
import pages.ErrorDialogPage
import pages.organDonation.OrganDonationBasePage
import pages.organDonation.OrganDonationChoicePage
import pages.organDonation.OrganDonationFaithModule
import pages.GpSessionError

open class OrganDonationStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var organDonationChoicePage: OrganDonationChoicePage

    lateinit var page: OrganDonationBasePage

    lateinit var homePage: HomePage

    private lateinit var errorDialogPage: ErrorDialogPage
    private lateinit var gpSessionError: GpSessionError

    @Given("^I am a (\\w+) user registered with organ donation to not donate my organs$")
    fun iAmRegisteredWithOrganDonationToNotDonateOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.existing.optOut()
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate all organs$")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.existing.optIn()
    }

    @Given("^I am a (\\w+) user registered with organ donation with an appointed representative$")
    fun iAmRegisteredWithOrganDonationWithAnAppointedRepresentative(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.existing.appointedRepresentative()
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate some organs$")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.existing.optInSome()
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate some organs, but not all are decided on$")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgansButNotAllDecidedOn(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.existing.optInSomeNotAllDecided()
    }

    @Given("^I am a (\\w+) user registered with organ donation with a decision to (.*) who wishes to withdraw$")
    fun iAmRegisteredWithOrganDonationAndWishToWithdraw(gpSystem: String, decision: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        val existing = factory.existing.setUpExistingDecisionForPatient(decision)
        factory.withdrawRegistration { request ->
            request.respondWithSuccess(existing.id)
        }
    }

    @Given("^I am a (\\w+) user registered with organ donation to donate all organs with a faith decision of '(.*)'$")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgansAndDecisionOfFaith(gpSystem: String, faith: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = OrganDonationFaithModule.getFaith(faith))
        factory.existing.optIn(demographics)
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register$")
    fun iAmNotRegisteredWithOrganDonationWishToRegister(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(HttpStatus.SC_NOT_FOUND) }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt out$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptOut(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration -> registration.optOut { request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and opt in$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptIn(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration -> registration.optIn { request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to opt in with '(.*)' faith " +
            "decision$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndOptInWithFaithDecision(gpSystem: String, faith:String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        val demographics = OrganDonationDemographics(faithDeclaration = OrganDonationFaithModule.getFaith(faith))
        factory.create { registration -> registration.optIn(demographics) {
            request -> request.respondWithSuccess("test") } }
    }

    @Given("^I am a (\\w+) user not registered with organ donation, who wishes to register and donate some organs$")
    fun iAmNotRegisteredWithOrganDonationWishToRegisterAndDonateSomeOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a -> a.respondWithError(HttpStatus.SC_NOT_FOUND) }
        factory.create { registration ->
            registration.some(OrganDonationRegistrationDataBuilder.someOrgansListUpdated())
            { request -> request.respondWithSuccess("test") }
        }
    }

    @When("^I click the Back link on an Organ Donation page$")
    fun iClickTheBackLinkOnThePage() {
        page.clickBackLinkButton()
    }

    @When("^I click the '(.*)' button on an Organ Donation page$")
    fun iClickTheButtonOnThePage(buttonText: String) {
        page.clickButton(buttonText)
    }

    @Then("^the internal Organ Donation page is displayed$")
    fun iAmOnTheInternalOrganDonationPage() {
        organDonationChoicePage.assertDisplayed()
    }

    @Then("^the '(.*)' button has the '(.*)' attribute$")
    fun theButtonHasTheAttribute(buttonText: String, attributeName: String) {
        page.assertButtonHasAttribute(buttonText, attributeName)
    }

    @Then("^Reference data is available for (.*)$")
    fun referenceDataIsAvailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)

        OrganDonationFactory(supplier).mockingClient.forOrganDonation.mock {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
        }
    }

    @Then("^I see appropriate try again error message for organ donation when there is no GP session$")
    fun iSeeAppropriateTryAgainErrorMessageWhenThereIsNoGpSessionForOrganDonation() {
        errorDialogPage
            .assertPageHeader("Sorry, there is a problem with organ donation")
            .assertPageTitle("Sorry, there is a problem with organ donation")
            .assertShutterParagraphText("You are not currently able to view or manage your organ donation decision.")
            .assertShutterParagraphText("This may be a temporary problem.")
    }

    @Then("^I see what I can do next with an organ donation error message$")
    fun iSeeOrganDonationUnavailableNoGpSession(){
        gpSessionError
            .assertPageHeader("Sorry, organ donation is unavailable")
            .assertParagraphText(
                "You are not currently able to view or manage your organ donation decision using the NHS App.")
            .assertLink("NHSApp.Enquiries@nhsbt.nhs.uk")
            .assertHeaderTag("Email", "h3")
    }
}
