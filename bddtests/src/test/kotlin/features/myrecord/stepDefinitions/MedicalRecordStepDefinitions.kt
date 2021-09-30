package features.myrecord.stepDefinitions

import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.factories.MyRecordFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ErrorDialogPage
import pages.MedicalRecordGpSessionError
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import utils.LinkedProfilesSerenityHelpers
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MedicalRecordStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps
    private lateinit var headerNative: HeaderNative
    private lateinit var errorDialogPage: ErrorDialogPage
    private lateinit var medicalRecordGpSessionError: MedicalRecordGpSessionError

    @Given("^the GP Practice has disabled summary care record functionality$")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        MyRecordFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled proxy access to summary care record functionality$")
    fun givenTheGPPracticeHasDisabledProxyAccessToSummaryCareRecordFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        MyRecordFactory.getForSupplier(gpSystem).disabled(ProxySerenityHelpers.getPatientOrProxy())
    }

    @Given("^the GP Practice has enabled all medical records for the patient$")
    fun givenTheGPPracticeHasEnabledAllMedicalRecordsForThePatient() {
        setupMedicalRecordAccessForPatient(dcrAccessEnabled = true)
    }

    @Given("^the GP Practice has disabled DCR access for the patient$")
    fun givenTheGPPracticeHasDisabledDcrAccessForThePatient() {
        setupMedicalRecordAccessForPatient(dcrAccessEnabled = false)
    }

    @When("^I enter url address for my record directly into the url$")
    fun whenIEnterUrlAddressForMyRecordDirectlyIntoTheUrl() {
        val fullUrl = "/health-records/gp-medical-record/"
        browser.browseTo(fullUrl)
    }

    @When("^I request my record data$")
    fun whenIRequestMyRecordData() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .myRecord.getMyRecord(patientId)
        Serenity.setSessionVariable(MyRecordResponse::class).to(result)
    }

    @When("^I navigate away from the medical record page$")
    fun iNavigateAwayFromTheMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.ADVICE)
    }

    @Then("^I see header text is Your GP health record$")
    fun thenISeeHeaderTextIsYourGPHealthRecord() {
        headerNative.waitForPageHeaderText("Your GP health record")
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    fun thenISeeMyRecordButtonOnTheNavBarIsHighlighted() {
        Assert.assertTrue(nav.hasSelectedTab(NavBarNative.NavBarType.YOUR_HEALTH))
    }

    @Then("^No navigation menu bar item will be selected$")
    fun thenNoNavigationMenuBarItemWillBeSelected() {
        if(headerNative.onMobile()) {
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.ADVICE))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.APPOINTMENTS))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.PRESCRIPTIONS))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.YOUR_HEALTH))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MESSAGES))
        }
    }

    @Then("^I see appropriate try again error message for gp medical record when there is no GP session$")
    fun iSeeAppropriateTryAgainErrorMessageWhenThereIsNoGpSessionForGpMedicalRecord() {
        errorDialogPage
                .assertParagraphText("You are not currently able to view your GP health record online.")
                .assertParagraphText("This may be a temporary problem.")
                .assertPageHeader("Sorry, there is a problem getting your GP health record")
                .assertPageTitle("Sorry, there is a problem getting your GP health record")
    }

    @Then("^I see what I can do next with a medical record error message and reference code '(.*)'$")
    fun iSeeMedicalRecordUnavailableNoGpSession(prefix: String){
        medicalRecordGpSessionError.assertMedicalRecordHeader()
                .assertReferenceCode(prefix)
                .assertParagraphText("If you need this information now, contact your GP surgery.")
                .assertParagraphText("For urgent medical advice, go to ")
                .assertReportAProblemLink()
    }

    private fun setupMedicalRecordAccessForPatient(dcrAccessEnabled: Boolean) {
        val supplier = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        if (dcrAccessEnabled) {
            MyRecordFactory.getForSupplier(supplier).enabledWithAllRecords(patient)
        } else {
            MyRecordFactory.getForSupplier(supplier).enabledWithNoDcrAccess(patient)
        }
    }
}
