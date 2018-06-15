package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.steps.MyRecordSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var recordSteps: MyRecordSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var homesteps: HomeSteps

    @Steps
    val mockingClient = MockingClient.instance
    val HTTP_EXCEPTION = "HttpException"

    @Given("the GP Practice has enabled summary care record functionality")
    fun givenTheGPPracticeHasEnabledSummaryCareRecordFunctionality() {

    }

    @Given("the GP Practice has disabled summary care record functionality")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionality() {

    }

    @When("^I click my record button on menu bar$")
    @Throws(Exception::class)
    fun i_click_my_record_button_on_menu_bar() {
        nav.select("MY_RECORD")
    }

    @Then("^I see record warning page opened$")
    @Throws(Exception::class)
    fun i_see_record_warning_page_opened() {
        Assert.assertTrue(recordSteps.isAgreePresent())
    }

    @Then("^I see header text is My medical record$")
    @Throws(Exception::class)
    fun i_see_header_text_is_My_medical_record() {
        Assert.assertEquals("My medical record", recordSteps.getHeaderText())
    }

    @Then("^I see your record may contain sensitive information message$")
    @Throws(Exception::class)
    fun i_see_your_record_may_contain_sensitive_information_message() {
        Assert.assertEquals("Your record may contain sensitive information.", recordSteps.warningText())
    }

    @Then("^I see sensitive information message highlighted yellow$")
    @Throws(Exception::class)
    fun i_see_sensitive_information_message_highlighted_yellow() {
        Assert.assertEquals("rgba(252, 237, 102, 1)", recordSteps.isWarningMsgHighlighted())
    }

    @Then("^I see list of sensitive data information$")
    @Throws(Exception::class)
    fun i_see_list_of_sensitive_data_information() {
        var expected = ArrayList<String>()
        expected.add("Personal data, such as your details, allergies and care preferences")
        expected.add("Medical history, such as your conditions and medications")
        expected.add("Test results that you may not have discussed with your doctor")
        expected.add("Clinical terms that you may not be familiar with")
        Assert.assertArrayEquals(expected.toArray(), recordSteps.getSensitiveList().toArray())
    }

    @Then("^I see agree and continue button$")
    @Throws(Exception::class)
    fun i_see_agree_and_continue_button() {
        Assert.assertTrue(recordSteps.isAgreePresent())
    }

    @Then("^I see back to home button$")
    @Throws(Exception::class)
    fun i_see_back_to_home_button() {
        Assert.assertTrue(recordSteps.isBackToHomePresent())
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    @Throws(Exception::class)
    fun i_see_my_record_button_on_the_nav_bar_is_highlighted() {
        Assert.assertTrue(nav.hasSelectedTab("MY_RECORD"))
    }

    @Given("^I am on the record warning page$")
    @Throws(Exception::class)
    fun i_am_on_the_record_warning_page() {
        nav.select("MY_RECORD")
    }

    @When("^I click agree and continue$")
    @Throws(Exception::class)
    fun i_click_agree_and_continue() {
        recordSteps.clickAgreeandContinue()
    }

    @Then("^the my record information screen is loaded$")
    @Throws(Exception::class)
    fun the_my_record_information_screen_is_loaded() {
        Assert.assertTrue(recordSteps.isOnMyRecordInfoPage())
    }

    @When("^I click the back to home button$")
    @Throws(Exception::class)
    fun i_click_the_back_to_home_button() {
        recordSteps.clickBacktoHome()
    }

    @Then("^I will return to the home page$")
    @Throws(Exception::class)
    fun i_will_return_to_the_home_page() {
        homesteps.assertHeaderVisible()
    }

    @Then("^No navigation menu bar item will be selected$")
    @Throws(Exception::class)
    fun no_navigation_menu_bar_item_will_be_selected() {
        Assert.assertTrue(!nav.hasSelectedTab("SYMPTOMS"))
        Assert.assertTrue(!nav.hasSelectedTab("APPOINTMENTS"))
        Assert.assertTrue(!nav.hasSelectedTab("PRESCRIPTIONS"))
        Assert.assertTrue(!nav.hasSelectedTab("MY_RECORD"))
        Assert.assertTrue(!nav.hasSelectedTab("MORE"))
    }

    @Then("^I see heading My details$")
    @Throws(Exception::class)
    fun i_see_heading_My_details() {
        Assert.assertEquals("My details", recordSteps.getMyDetailsLabelText())
    }

    @Then("^I see patient information details$")
    @Throws(Exception::class)
    fun i_see_patient_information_details() {
        Assert.assertEquals("Name", recordSteps.getNameLabelText())
        Assert.assertEquals("Date of birth", recordSteps.getDOBLabelText())
        Assert.assertEquals("Sex", recordSteps.getSexLabelText())
        Assert.assertEquals("Address", recordSteps.getAddressLabelText())
        Assert.assertEquals("NHS number", recordSteps.getNHSNumberLabelText())
    }

    @Given("^I am on my record information page$")
    @Throws(Exception::class)
    fun i_am_on_my_record_information_page() {
        browser.goToApp()
        login.asDefault()
        nav.select("MY_RECORD")
        recordSteps.clickAgreeandContinue()
        Assert.assertTrue(recordSteps.isOnMyRecordInfoPage())
    }

    @When("^I click My details heading$")
    @Throws(Exception::class)
    fun i_click_My_details_heading() {
        recordSteps.clickMyDetails()
    }

    @Then("^I do not see patient information details$")
    @Throws(Exception::class)
    fun i_do_not_see_patient_information_details() {
        Assert.assertFalse(recordSteps.isNameVisible())
    }

    @When("I get the users my record data")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I click the Allergies and Adverse Reactions section$")
    @Throws(Exception::class)
    fun i_click_the_Allergies_and_Adverse_Reactions_section() {
        recordSteps.clickAllergiesAndAdverseReactionsSection()
    }

    @Then("^I see the Allergies and Adverse Reactions heading$")
    @Throws(Exception::class)
    fun i_see_the_Allergies_and_Adverse_Reactions_heading() {
        Assert.assertEquals("Allergies and Adverse Reactions", recordSteps.getAllergiesAndAdverseReactionsHeaderText())
    }

    @Then("^I see the Allergies and Adverse Reactions section collapsed$")
    @Throws(Exception::class)
    fun i_see_the_Allergies_and_Adverse_Reactions_section_collapsed() {
        Assert.assertFalse(recordSteps.isAllergiesTextMsgVisible())
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    @Throws(Exception::class)
    fun i_see_Service_not_offered_by_GP_or_to_specific_user_or_access_revoked_warning_message() {
        Assert.assertEquals("Service not offered by GP or to specific user or access revoked", recordSteps.getAccessRevokedMessage())
    }

    @Then("^I see a message indicating I have no allergies$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_I_have_no_allergies() {
        Assert.assertEquals("No information recorded for this section", recordSteps.getAllergyMessage())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    @Throws(Exception::class)
    fun i_see_one_or_more_drug_type_allergies_record_displayed() {
        Assert.assertEquals("Drug Allergy", recordSteps.getAllergyMessage())
    }

    @Then("^I see one or more non drug type allergies record displayed$")
    @Throws(Exception::class)
    fun i_see_one_or_more_non_drug_type_allergies_record_displayed() {
        Assert.assertEquals("non Drug Allergy", recordSteps.getAllergyMessage())
    }

    @Given("^I see heading Acute medications$")
    @Throws(Exception::class)
    fun i_see_heading_acute_medications() {
        Assert.assertEquals("Acute medications", recordSteps.getAcuteMedicationsHeaderText())
    }

    @When("^I click acute medications$")
    @Throws(Exception::class)
    fun i_click_acute_medications() {
        recordSteps.clickAcuteMedications()
    }

    @Then("^I see acute medication information$")
    @Throws(Exception::class)
    fun i_see_acute_medication_information() {
        Assert.assertEquals("Medications", recordSteps.getAcuteMedications())
    }
}