package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.myrecord.MyRecordWarningPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import utils.SerenityHelpers

open class MyRecordWarningStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var myRecordStepDefinitions: MyRecordStepDefinitions

    lateinit var headerNative: HeaderNative

    lateinit var myRecordWarningPage: MyRecordWarningPage

    @Given("^I am on the record warning page$")
    fun givenIAmOnTheRecordWarningPage() {
        browser.goToApp()
        login.using(SerenityHelpers.getPatient())
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @When("^I click continue$")
    fun whenIClickContinue() {
        myRecordWarningPage.clickWarningContinue()
    }

    @When("^I click the back to home$")
    fun whenIClickTheBackToHome() {
        myRecordWarningPage.clickBacktoHome()
    }

    @Then("^I see record warning page opened$")
    fun thenISeeRecordWarningPageOpened() {
        myRecordStepDefinitions.thenISeeHeaderTextIsMyMedicalRecord()
        thenISeeContinue()
        thenISeeBackToHome()
    }

    @Then("^I see the my record warning page")
    fun iSeeTheMyRecordWarningPage() {
        thenISeeRecordWarningPageOpened()
        myRecordStepDefinitions.thenISeeHeaderTextIsMyMedicalRecord()
        thenISeeWhatMedicalInformationWillBeShown()
        theISeeYourRecordMayContainSensitiveInformationMessage()
        thenISeeListOfSensitiveDataInformation()
        thenISeeContinue()
        thenISeeBackToHome()
        myRecordStepDefinitions.thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }

    @Then("^I see your record may contain sensitive information message$")
    fun theISeeYourRecordMayContainSensitiveInformationMessage() {
        assertEquals("Your record may contain sensitive information. If someone is pressuring you for this" +
                " information, contact your GP surgery immediately.", myRecordWarningPage.warningText())
    }

    @Then("^I see what medical information will be shown$")
    fun thenISeeWhatMedicalInformationWillBeShown() {
        assertEquals("Your record shows personal data, such as your details, allergies and medications.",
                myRecordWarningPage.getShownInformationDescription())
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
