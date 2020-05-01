package features.linkedProfiles.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.GpPracticeAccessSettingsFactory
import features.myrecord.factories.MyRecordFactory
import features.prescriptions.stepDefinitions.PrescriptionsDataSetup
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import mocking.nhsAzureSearchService.nhsAzureSearchOrganisationByOdsCodeRequestBody
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import mockingFacade.linkedProfiles.FeaturesEnabledFacade
import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import models.patients.PatientHandler
import models.linkedProfiles.LinkedProfileOption
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
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
import utils.LinkedProfilesSerenityHelpers
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import java.util.*

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

    private val mockingClient = MockingClient.instance

    @Given("^I am an? (.*) user with linked profiles$")
    fun iAmAUserWithLinkedProfiles(gpSystem:String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = PatientHandler.getForSupplier(supplier).getPatientWithLinkedProfiles()
        SerenityHelpers.setGpSupplier(supplier)
        setup(patient, supplier)
    }

    @Given("^I am logged in as a (.*) user with linked profiles and appointments provider (.*)$")
    fun iAmLoggedInWithLinkedProfilesAndAppointmentsProvider(gpSystem: String, provider: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = PatientHandler.getForSupplier(supplier).getPatientWithLinkedProfiles()
        PatientHandler.getForSupplier(supplier).setOdsCode(patient, provider)
        SerenityHelpers.setGpSupplier(supplier)
        setupAndLogIn(patient, supplier)
    }

    @Given("^I am logged in as a TPP user with linked profiles but no access to " +
            "core services and appointments provider (.*)$")
    fun iAmLoggedInWithLinkedProfilesButNoAccessToCoreServicesAndAppointmentsProvider
            (provider: String) {
        val supplier = Supplier.TPP
        val patient = PatientHandler.getForSupplier(supplier).getPatientWithLinkedProfiles()
        PatientHandler.getForSupplier(supplier).setOdsCode(patient, provider)
        SerenityHelpers.setGpSupplier(supplier)
        PrescriptionsDataSetup.disabled(patient.linkedAccounts.toList()[0], Supplier.TPP)
        PrescriptionsDataSetup.disabled(patient, Supplier.TPP)
        setupAndLogIn(patient, supplier)
    }

    @Given("^I am logged in as a (.*) user with no linked profiles")
    fun iAmLoggedInWithNoLinkedProfiles(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = PatientHandler.getForSupplier(supplier).getPatientWithNoLinkedProfiles()
        SerenityHelpers.setGpSupplier(supplier)

        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory
                .getForSupplier(supplier)
                .createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)

        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabled(patient)

        browser.goToApp()
        login.using(patient)
    }

    @Given("I have switched to a linked profile")
    fun iHaveSwitchedToALinkedProfile(){
        iSelectTheLinkedProfilesLink()
        iSelectALinkedProfile()
        iClickTheSwitchToThisProfileButtonForTheProxyUser()
    }

    @Given("^I click on the Appointments link on the header$")
    fun iClickOnAppointmentsLinkInHeader() {
        webHeader.clickAppointmentsPageLink()
    }

    @When("^I click on the My Record link on the header$")
    fun iClickOnMyRecordLinkInHeader() {
        webHeader.clickMyRecordPageLink()
    }

    private fun setupAndLogIn(patient: Patient, gpSystem: Supplier) {
        setup(patient, gpSystem)
        browser.goToApp()
        login.using(patient)
    }

    private fun setup(patient: Patient, gpSystem: Supplier) {
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory
                .getForSupplier(gpSystem)
                .createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
        DemographicsFactory
                .getForSupplier(gpSystem)
                .enableForPatientProxyAccounts(patient)
    }

    @Given("^the GP Practice has enabled all medical records for the proxy patient$")
    fun theGpPracticeHasEnabledAllMedicalRecordsForTheProxyPatient() {
        MyRecordFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithAllRecords(ProxySerenityHelpers.getPatientOrProxy())
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
        home.linkedProfilesLink.click()
    }

    @And("^linked profiles are displayed$")
    fun linkedProfilesAreDisplayed() {
        val patient = SerenityHelpers.getPatient()
        val displayedLinkedProfiles = linkedProfilesPage.getDisplayedLinkedProfiles()
        val userLinkedProfiles = patient.linkedAccounts

        checkDisplayedValuesAreCorrect(displayedLinkedProfiles, userLinkedProfiles)
    }

    private fun iSelectALinkedProfileWithFeatures(featuresEnabledFacade: FeaturesEnabledFacade) {
        val patient = SerenityHelpers.getPatient()
        val gpSystem = SerenityHelpers.getGpSupplier()

        // just select the first one
        val linkedAccount = patient.linkedAccounts.toList()[0]

        when (gpSystem) {
            Supplier.TPP -> LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.set(linkedAccount.formattedFullName())
            Supplier.EMIS -> {
                LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.set(linkedAccount.name.firstName)
                GpPracticeAccessSettingsFactory.getForSupplier(gpSystem).enabledViaProxy(
                        callingPatient = patient,
                        actingOnBehalfOf = linkedAccount,
                        featuresEnabled = featuresEnabledFacade)
            }
            else -> throw IllegalArgumentException("$gpSystem not supported in Proxy mode")
        }

        val gpPractice = NhsAzureSearchData.generateOrganisationData(1)
        mockingClient.forAzure.forSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(
                    nhsAzureSearchOrganisationByOdsCodeRequestBody(
                            odsCode = "${linkedAccount.odsCode}")
            )
                    .respondWithSuccess(gpPractice)
        }

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
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            symptomsShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            symptomsShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the settings shutter page is displayed$")
    fun theSettingsShutterPageIsDisplayed() {
        settingsShutterPage.isLoaded()
        settingsShutterPage.assertText()
    }

    @Then("^the prescriptions shutter page is displayed$")
    fun thePrescriptionsShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            prescriptionsShutterPage.isLoaded(selectedProfile.profile.formattedFullName())
            prescriptionsShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            prescriptionsShutterPage.isLoaded(selectedProfile.profile.name.firstName)
            prescriptionsShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the appointments shutter page is displayed$")
    fun theAppointmentsShutterPageIsDisplayed() {

        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE
                .getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            appointmentsShutterPage.isLoaded(selectedProfile.profile.formattedFullName())
            appointmentsShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            appointmentsShutterPage.isLoaded(selectedProfile.profile.name.firstName)
            appointmentsShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the medical record shutter page is displayed$")
    fun theMedicalRecordShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            medicalRecordShutterComponent.assertText(selectedProfile.profile,
                    selectedProfile.profile.formattedFullName())
        } else {
            medicalRecordShutterComponent.assertText(selectedProfile.profile,
                    selectedProfile.profile.name.firstName)
        }
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
                    "Linked profile age did not match",
                    patient.age.formattedAge(),
                    displayedLinkedProfiles[index].age)
        }
    }
}
