package features.linkedProfiles.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.GpPracticeAccessSettingsFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.nhsAzureSearchService.nhsAzureSearchOrganisationByOdsCodeRequestBody
import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import models.linkedProfiles.LinkedProfileOption
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
import pages.isVisible
import pages.linkedProfiles.LinkedProfileSummaryPage
import pages.linkedProfiles.LinkedProfilesPage
import pages.text
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import java.util.ArrayList

class LinkedProfilesStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps

    lateinit var home: HomePage

    private lateinit var linkedProfilesPage: LinkedProfilesPage
    private lateinit var linkedProfileSummaryPage: LinkedProfileSummaryPage

    val mockingClient = MockingClient.instance

    @Given("^I am logged in as a (.*) user with linked profiles$")
    fun iAmLoggedInWithLinkedProfiles(gpSystem: String) {
        val patient = Patient.getPatientWithLinkedProfiles(gpSystem)
        SerenityHelpers.setGpSupplier(gpSystem)
        setupWithLinkedAccountsAndLogIn(patient, gpSystem)
    }

    private fun setupWithLinkedAccountsAndLogIn(patient: Patient, gpSystem: String) {
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory
                .getForSupplier(gpSystem, mockingClient)
                .createFor(patient)

        browser.goToApp()
        login.using(patient)
    }

    @Then("^I see the linked profiles link$")
    fun iSeeLinkedProfilesLink() {
        Assert.assertTrue("Linked profiles link not visible", home.linkedProfilesLink.isVisible)
    }

    @Then("^the linked profiles page is displayed$")
    fun theLinkedProfilesPageIsDisplayed() {
        linkedProfilesPage.isLoaded()
    }

    @Then("^I select the linked profiles link from the home page$")
    fun iSelectTheLinkedProfilesLink() {
        // make sure demographics is setup for each linked profile
        val patient = SerenityHelpers.getPatient()
        patient.linkedAccounts.forEach {
            DemographicsFactory
                    .getForSupplier(SerenityHelpers.getGpSupplier())
                    .enabledViaProxy(patient, it)
        }

        home.linkedProfilesLink.click()
    }

    @And("^linked profiles are displayed$")
    fun linkedProfilesAreDisplayed() {
        val patient = SerenityHelpers.getPatient()
        val displayedLinkedProfiles = linkedProfilesPage.getDisplayedLinkedProfiles()
        val userLinkedProfiles = patient.linkedAccounts

        checkDisplayedValuesAreCorrect(displayedLinkedProfiles, userLinkedProfiles)
    }

    @Then("^I select a linked profile$")
    fun iSelectALinkedProfile() {
        val patient = SerenityHelpers.getPatient()
        val gpSystem = SerenityHelpers.getGpSupplier()

        // just select the first one
        val linkedAccount = patient.linkedAccounts.toList()[0]

        val gpPractice = NhsAzureSearchData.generateOrganisationData(1)
        mockingClient.forAzure.forSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(
                    nhsAzureSearchOrganisationByOdsCodeRequestBody(
                            odsCode = "${linkedAccount.odsCode}")
            )
                    .respondWithSuccess(gpPractice)
        }

        GpPracticeAccessSettingsFactory.getForSupplier(gpSystem).enabledViaProxy(
                callingPatient = patient,
                actingOnBehalfOf = linkedAccount
        )

        LinkedProfilesSerenityHelpers.SELECTED_PROFILE.set(
                LinkedProfileFacade(
                        linkedAccount,
                        gpPractice.value[0].OrganisationName)
        )

        linkedProfilesPage.selectLinkedProfile(linkedAccount.formattedFullName())
    }

    @Then("details for the selected linked profile are displayed")
    fun detailsForTheSelectedLinkedProfileAreDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()

        linkedProfileSummaryPage.isLoaded(selectedProfile.profile.formattedFullName())

        val displayedLinkedProfile = linkedProfileSummaryPage.getDisplayedLinkedProfileDetail()

        Assert.assertEquals(
                "Linked profile NHS number did not match",
                selectedProfile.profile.formattedNHSNumber(),
                displayedLinkedProfile.nhsNumber)

        Assert.assertEquals(
                "Linked profile date of birth did not match",
                selectedProfile.profile.formattedDateOfBirth(),
                displayedLinkedProfile.dateOfBirth)

        Assert.assertEquals(
                "Linked profile GP practice name did not match",
                selectedProfile.gpPracticeName,
                displayedLinkedProfile.gpPracticeName)
    }

    @When("I click the Switch to my profile button")
    fun iClickTheSwitchToMyProfileButton() {
        linkedProfileSummaryPage.switchProfileButton.click()
    }

    @Then("^the yellow banner contains details for the user I am acting on behalf of$")
    fun theYellowBannerContainsDetailsForTheUserIAmActingOnBehalfOf() {
        val expectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val bannerText = home.banner.text

        Assert.assertTrue("Banner does not contain expected descriptive text",
                bannerText.contains("Acting on behalf of"))

        Assert.assertTrue("Banner does not contain expected name",
                bannerText.contains(expectedProfile.profile.formattedFullName()))
    }

    private fun checkDisplayedValuesAreCorrect(
            displayedLinkedProfiles: ArrayList<LinkedProfileOption>,
            linkedPatients: Set<Patient>)
    {
        Assert.assertEquals(
                "Number of linked profiles displayed is not correct",
                displayedLinkedProfiles.size,
                linkedPatients.size)

        linkedPatients.forEachIndexed { index, patient ->
            Assert.assertEquals(
                    "Linked profile name did not match",
                    patient.formattedFullName(),displayedLinkedProfiles[index].name)

            Assert.assertEquals(
                    "Linked profile date of birth did not match",
                    patient.formattedDateOfBirth(),
                    displayedLinkedProfiles[index].dateOfBirth)
        }
    }
}
