package features.linkedProfiles.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.GpPracticeAccessSettingsFactory
import features.myrecord.factories.MyRecordFactory
import features.prescriptions.factories.PrescriptionsFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.nhsAzureSearchService.nhsAzureSearchOrganisationByOdsCodeRequestBody
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import mockingFacade.linkedProfiles.FeaturesEnabledFacade
import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import models.linkedProfiles.LinkedProfileOption
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
import pages.isPresent
import pages.isVisible
import pages.linkedProfiles.LinkedProfileSummaryPage
import pages.linkedProfiles.LinkedProfilesPage
import pages.linkedProfiles.shutterPages.AppointmentsShutterPage
import pages.linkedProfiles.shutterPages.MedicalRecordShutterComponent
import pages.linkedProfiles.shutterPages.PrescriptionsShutterPage
import pages.linkedProfiles.shutterPages.SettingsShutterPage
import pages.linkedProfiles.shutterPages.SymptomsShutterPage
import pages.navigation.WebHeader
import pages.text
import utils.GlobalSerenityHelpers
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
    private lateinit var prescriptionsShutterPage: PrescriptionsShutterPage
    private lateinit var appointmentsShutterPage: AppointmentsShutterPage
    private lateinit var settingsShutterPage: SettingsShutterPage
    private lateinit var symptomsShutterPage: SymptomsShutterPage
    private lateinit var medicalRecordShutterComponent: MedicalRecordShutterComponent
    private lateinit var webHeader: WebHeader

    val mockingClient = MockingClient.instance

    @Given("^I am logged in as a (.*) user with linked profiles and appointments provider (.*)$")
    fun iAmLoggedInWithLinkedProfilesAndAppointmentsProvider(gpSystem: String, provider: String) {
        val patient = Patient.getPatientWithLinkedProfiles(gpSystem)
        Patient.setOdsCodeBasedOnAppointmentsProvider(patient, provider)
        SerenityHelpers.setGpSupplier(gpSystem)
        setupWithLinkedAccountsAndLogIn(patient, gpSystem)
    }

    @Given("^I click on the Appointments link on the header$")
    fun iClickOnAppointmentsLinkInHeader() {
        webHeader.clickAppointmentsPageLink()
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

    private fun iSelectALinkedProfileWithFeatures(featuresEnabled: FeaturesEnabledFacade) {
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
                actingOnBehalfOf = linkedAccount,
                featuresEnabled = FeaturesEnabledFacade(
                        appointmentsEnabled = featuresEnabled.appointmentsEnabled,
                        prescribingEnabled = featuresEnabled.prescribingEnabled,
                        medicalRecordEnabled = featuresEnabled.medicalRecordEnabled)
        )

        LinkedProfilesSerenityHelpers.SELECTED_PROFILE.set(
                LinkedProfileFacade(
                        linkedAccount,
                        gpPractice.value[0].OrganisationName)
        )

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                GlobalSerenityHelpers.SWITCHED_LINKED_ACCOUNT, linkedAccount)

        linkedProfilesPage.selectLinkedProfile(linkedAccount.formattedFullName())
    }

    @Then("^I select a linked profile$")
    fun iSelectALinkedProfile() {
        iSelectALinkedProfileWithFeatures(
                FeaturesEnabledFacade(
                        appointmentsEnabled = true,
                        prescribingEnabled = false,
                        medicalRecordEnabled = true)
        )
    }

    @Then("^I select a linked profile with " +
            "appointments enabled (.*), " +
            "prescriptions enabled (.*) and " +
            "medical record enabled (.*)$")
    fun iSelectALinkedProfileWithFeaturesDisabled(
            appointments: Boolean,
            prescriptions: Boolean,
            medicalRecord: Boolean) {
        iSelectALinkedProfileWithFeatures(
                FeaturesEnabledFacade(
                        appointmentsEnabled = appointments,
                        prescribingEnabled = prescriptions,
                        medicalRecordEnabled = medicalRecord)
        )

        val patient = SerenityHelpers.getPatient()
        val actingOnBehalfOf = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()

        if (!prescriptions) {
            PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .disableForProxy(patient, actingOnBehalfOf.profile)
        }

        if (!appointments) {
            MyAppointmentsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .disableForProxy(actingOnBehalfOf.profile)
        }

        if (!medicalRecord) {
            MyRecordFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .disabledForProxy(patient, actingOnBehalfOf.profile)
        }
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

    @When("I click the Switch to this profile button for the proxy user")
    fun iClickTheSwitchToThisProfileButtonForTheProxyUser() {
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

    @Then("^the symptoms shutter page is displayed$")
    fun theSymptomsShutterPageIsDisplayed() {
        symptomsShutterPage.isLoaded()
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        symptomsShutterPage.assertSummaryText(selectedProfile.profile.firstName)
        symptomsShutterPage.assertSwitchText()
    }

    @Then("^the settings shutter page is displayed$")
    fun theSettingsShutterPageIsDisplayed() {
        settingsShutterPage.isLoaded()

        Assert.assertFalse(
                "Summary text should not be visible",
                settingsShutterPage.summaryText.isPresent)

        settingsShutterPage.assertSwitchText()
    }

    @Then("^the prescriptions shutter page is displayed$")
    fun thePrescriptionsShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        prescriptionsShutterPage.isLoaded(selectedProfile.profile.firstName)
        prescriptionsShutterPage.assertSummaryText(selectedProfile.profile.firstName)
        prescriptionsShutterPage.assertSwitchText()
    }

    @Then("^the appointments shutter page is displayed$")
    fun theAppointmentsShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        appointmentsShutterPage.isLoaded(selectedProfile.profile.firstName)
        appointmentsShutterPage.assertSummaryText(selectedProfile.profile.firstName)
        appointmentsShutterPage.assertSwitchText()
    }

    @Then("^the medical record shutter page is displayed$")
    fun theMedicalRecordShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        medicalRecordShutterComponent.assertSubHeaderText(selectedProfile.profile.firstName)
        medicalRecordShutterComponent.assertSummaryText(selectedProfile.profile.firstName)
        medicalRecordShutterComponent.assertSwitchText()
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
