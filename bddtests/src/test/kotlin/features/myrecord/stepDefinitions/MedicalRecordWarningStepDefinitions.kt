package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.myrecord.MyRecordWarningPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse

open class MedicalRecordWarningStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var medicalRecordStepDefinitions: MedicalRecordStepDefinitions

    private lateinit var myRecordWarningPage: MyRecordWarningPage

    private lateinit var headerNative: HeaderNative

    @Given("^I am on the Medical Record Warning page$")
    fun givenIAmOnTheRecordWarningPage() {
        val patient = SerenityHelpers.getPatient()
        browser.goToApp()

        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enableForPatientProxyAccounts(patient)

        login.using(patient)
        home.waitForLoginToCompleteSuccessfully()
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @When("^I click continue$")
    fun whenIClickContinue() {
        myRecordWarningPage.clickWarningContinue()
    }

    @When("^I click back to home$")
    fun whenIClickBackToHome() {
        myRecordWarningPage.clickBacktoHome()
    }

    @Then("^the field indicating supplier is set$")
    fun thenTheFlagIndicatingSupplierIsSetTo() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(SerenityHelpers.getGpSupplier().toString().toUpperCase(), result.response.supplier.toUpperCase())
    }

    @Then("^I return to my medical record page$")
    fun thenIReturnToMyMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @Then("^I see record warning page opened$")
    fun thenISeeRecordWarningPageOpened() {
        headerNative.waitForPageHeaderText("Your GP medical record")
        thenISeeContinue()
        thenISeeBackToHome()
    }

    @Then("^I see the Medical Record Warning page")
    fun iSeeTheMedicalRecordWarningPage() {
        headerNative.waitForPageHeaderText("Your GP medical record")
        theISeeYourRecordMayContainSensitiveInformationMessage()
        thenISeeListOfSensitiveDataInformation()
        thenISeeContinue()
        thenISeeBackToHome()
        medicalRecordStepDefinitions.thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }

    @Then("^I see your record may contain sensitive information message$")
    fun theISeeYourRecordMayContainSensitiveInformationMessage() {
        print(myRecordWarningPage.warningText())
        assertEquals("Your record may contain sensitive information. If someone is pressuring you for this" +
                " information, contact your GP surgery immediately.\nYou have a legal right to access the information" +
                " in your record.", myRecordWarningPage.warningText())
    }

    @Then("^I see list of sensitive data information$")
    fun thenISeeListOfSensitiveDataInformation() {
        val expected = ArrayList<String>()
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
        assertArrayEquals(expected.toArray(), myRecordWarningPage.getSensitiveList().toArray())
    }

    @Then("^I see continue$")
    fun thenISeeContinue() {
        assertTrue("isContinuePresent", myRecordWarningPage.isContinuePresent())
    }

    @Then("^I see back to home$")
    fun thenISeeBackToHome() {
        assertTrue("isBackToHomePresent", myRecordWarningPage.isBackToHomePresent())
    }
}
