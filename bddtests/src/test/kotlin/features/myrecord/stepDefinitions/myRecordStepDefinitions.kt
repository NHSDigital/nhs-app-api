package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.steps.MyRecordSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.data.myrecord.*
import mocking.defaults.MockDataPopulate
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.emis.demographics.PatientIdentifier
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.Error
import models.Patient
import java.time.OffsetDateTime

open class MyRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

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

    @Given("^the my record wiremocks are initialised for (.*)$")
    fun givenMyRecordWiremocksAreInitialisedfor(getService: String) {
        setPatientToDefaultFor(getService)

        when (getService) {
            "EMIS" -> {
                MockDataPopulate(mockingClient).populate()

                mockingClient.forEmis {
                    testResultsRequest(patient).respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
                }

                mockingClient.forEmis {
                    immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
                }

                mockingClient.forEmis {
                    allergiesRequest(patient).respondWithSuccess(AllergiesData.getEmisDefaultAllergyModel())
                }

                mockingClient.forEmis {
                    medicationsRequest(patient).respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
                }

                mockingClient.forEmis {
                    problemsRequest(patient).respondWithSuccess(ProblemsData.getDefaultProblemModel())
                }

                mockingClient.forEmis {
                    consultationsRequest(MockDefaults.patient).respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
                }
            }
            "TPP" -> {
                MockDataPopulate(mockingClient).populate()
                CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
                mockingClient.forTpp {
                    viewPatientOverviewPost(patient.tppUserSession!!).respondWithSuccess(ViewPatientOverviewData.getTppViewPatientOverviewData())
                }

                mockingClient.forTpp {
                    patientRecordRequest(patient.tppUserSession!!).respondWithSuccess(TppDcrData.getDefaultTppDcrData())
                }

                val startDate = OffsetDateTime.now()
                val endDate = startDate.minusDays(60)

                mockingClient.forTpp {
                    testResultsViewRequest(patient.tppUserSession!!, startDate, endDate).respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
                }
            }
        }
    }

    @Given("the GP Practice has enabled summary care record functionality")
    fun givenTheGPPracticeHasEnabledSummaryCareRecordFunctionality() {

    }

    @Given("the GP Practice has disabled summary care record functionality for (.*)")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionalityfor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    allergiesRequest(patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithError(Error("6", "Requested record access is disabled by the practice", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    @When("I click a test result$")
    @Throws(Exception::class)
    fun i_click_a_test_result() {
        recordSteps.clickTestResult()
    }

    @When("^I click my record button on menu bar$")
    @Throws(Exception::class)
    fun i_click_my_record_button_on_menu_bar() {
        nav.select("MY_RECORD")
    }

    @When("^I enter url address for my record directly into the url$")
    @Throws(Exception::class)
    fun i_enter_url_address_for_my_record_directly_into_the_url() {
        val fullUrl = Config.instance.url + "/my-record"
        browser.browseTo(fullUrl)
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
        Assert.assertEquals("Your record may contain sensitive information", recordSteps.warningText())
    }

    @Then("^I see list of sensitive data information$")
    @Throws(Exception::class)
    fun i_see_list_of_sensitive_data_information() {
        var expected = ArrayList<String>()
        expected.add("personal data, such as your details, allergies and medications")
        expected.add("clinical terms that you may not be familiar with")
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
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
        browser.goToApp()
        login.using(this.patient)
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
        login.using(this.patient)
        nav.select("MY_RECORD")
        recordSteps.clickAgreeandContinue()
        Assert.assertTrue(recordSteps.isOnMyRecordInfoPage())
        Assert.assertTrue(recordSteps.canSeeClinicalAbbreviationsLink())
    }

    @Then("^I can see the clinical abbreviations link$")
    @Throws(Exception::class)
    fun i_can_see_the_clinical_abbreviations_link() {
        Assert.assertTrue(recordSteps.canSeeClinicalAbbreviationsLink())
    }

    @Then("^I click the clinical abbreviations link$")
    @Throws(Exception::class)
    fun i_click_the_clinical_abbreviations_link() {
        recordSteps.clickClinicalAbbreviationsLink()
    }

    @When("^I click My details heading$")
    @Throws(Exception::class)
    fun i_click_My_details_heading() {
        recordSteps.clickMyDetails()
    }

    @Then("^I do not see patient information details$")
    @Throws(Exception::class)
    fun i_do_not_see_patient_information_details() {
        Assert.assertFalse("Name field was visible.", recordSteps.isNameVisible())
    }

    @When("^I get the users my record data$")
    fun whenIGetTheUsersMyRecordData() {
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
        Assert.assertEquals("Allergies and adverse reactions", recordSteps.getAllergiesAndAdverseReactionsHeaderText())
    }

    @Then("^I see the Allergies and Adverse Reactions section collapsed$")
    @Throws(Exception::class)
    fun i_see_the_Allergies_and_Adverse_Reactions_section_collapsed() {
        Assert.assertFalse(recordSteps.isAllergiesTextMsgVisible())
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    @Throws(Exception::class)
    fun i_see_Service_not_offered_by_GP_or_to_specific_user_or_access_revoked_warning_message() {
        Assert.assertEquals("You don’t currently have online access to your medical record\nPlease contact your GP surgery for more information.", recordSteps.getSummaryCareNoAccessMessage())
    }

    @Then("^I see a message indicating I have no allergies$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_I_have_no_allergies() {
        Assert.assertEquals("No information recorded", recordSteps.getNoAllergyMessage())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    @Throws(Exception::class)
    fun i_see_one_or_more_drug_type_allergies_record_displayed() {
        Assert.assertEquals(2, recordSteps.getAllergyCount())
        var expected = ArrayList<String>()
        for (i in 1..2) {
            expected.add(AllergiesData.TERM)
        }

        Assert.assertArrayEquals(expected.toArray(), recordSteps.getAllergyMessages().toArray())
    }

    @Then("^I see 5 allergies with different date formats$")
    @Throws(Exception::class)
    fun i_see_five_allergies_with_different_date_formats() {

        Assert.assertEquals(5, recordSteps.getAllergyCount())
        Assert.assertTrue(recordSteps.getAllergyDates().contains("15 May 2018"))
        Assert.assertTrue(recordSteps.getAllergyDates().contains("May 2018"))
        Assert.assertTrue(recordSteps.getAllergyDates().contains("2018"))
        Assert.assertTrue(recordSteps.getAllergyDates().contains("15 May 2018 09:52"))
    }

    @Then("^I see one or more non drug type allergies record displayed$")
    @Throws(Exception::class)
    fun i_see_one_or_more_non_drug_type_allergies_record_displayed() {
        Assert.assertEquals("non Drug Allergy", recordSteps.getAllergyMessage())
    }

    @Given("^I see heading Acute medications$")
    @Throws(Exception::class)
    fun i_see_heading_acute_medications() {
        Assert.assertEquals("Acute (short-term) medications", recordSteps.getAcuteMedicationsHeaderText())
    }

    @When("^I click acute medications$")
    @Throws(Exception::class)
    fun i_click_acute_medications() {
        recordSteps.clickAcuteMedications()
    }

    @Then("^I see acute medication information$")
    @Throws(Exception::class)
    fun i_see_acute_medication_information() {
        Assert.assertTrue(recordSteps.isAcuteMedicationsAvailable())
    }

    @When("^I click the test result section$")
    @Throws(Exception::class)
    fun i_click_the_test_result_section() {
        recordSteps.clickTestResultsSection()
    }

    @Then("^I see one test result with one value$")
    @Throws(Exception::class)
    fun i_see_one_test_result_with_one_value() {
        Assert.assertEquals(1, recordSteps.getTestResultCount())
        Assert.assertEquals(1, recordSteps.getTestResultChildCount())
        //Assert.assertEquals("No information recorded for this section", recordSteps.getTestResultMsg())
    }

    @Then("^I see one test result with one value and a range$")
    @Throws(Exception::class)
    fun i_see_one_test_result_with_one_value_and_a_range() {
        Assert.assertEquals(1, recordSteps.getTestResultCount())
        Assert.assertEquals(1, recordSteps.getTestResultChildCount())
    }

    @Then("^I see one test result with multiple child values$")
    @Throws(Exception::class)
    fun i_see_one_test_result_with_multiple_child_values() {
        Assert.assertTrue(recordSteps.getTestResultCount() >= 1)
        Assert.assertTrue(recordSteps.getTestResultChildCount() > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges$")
    @Throws(Exception::class)
    fun i_see_test_results_with_multiple_child_values_some_of_which_ave_ranges() {
        Assert.assertTrue(recordSteps.getTestResultCount() >= 1)
        Assert.assertTrue(recordSteps.getTestResultChildCount() > 1)
    }

    @Then("^I see the test result heading for (.*)$")
    @Throws(Exception::class)
    fun i_see_the_test_result_heading(getService: String) {
        when (getService) {
            "TPP" -> {
                Assert.assertEquals("Test results (past 6 months)", recordSteps.getTestResultsHeaderText())
            }
            else -> {
                Assert.assertEquals("Test results", recordSteps.getTestResultsHeaderText())
            }
        }
    }

    @Then("^I see the test result section collapsed$")
    @Throws(Exception::class)
    fun i_see_the_test_result_section_collapsed() {
        Assert.assertFalse(recordSteps.isTestResultsTextMsgVisible())
    }

    @Then("^I see test result information$")
    @Throws(Exception::class)
    fun i_see_test_result_information() {
        Assert.assertTrue(recordSteps.isTestResultsTextMsgVisible())
    }

    @Then("^I see a message indicating that I have no access to view test result$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_test_result() {
        Assert.assertEquals("You don't currently have access to this section", recordSteps.getTestResultsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for this section$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_this_section() {
        Assert.assertEquals("No information recorded", recordSteps.getTestResultsMessage())
    }

    @Then("^I see an error occured message$")
    @Throws(Exception::class)
    fun i_see_an_error_occured_message() {
        Assert.assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getTestResultsMessage())
    }

    @Given("^I see heading Current repeat medications$")
    @Throws(Exception::class)
    fun i_see_heading_Current_repeat_medications() {

    }

    @When("^I click current repeat medications$")
    @Throws(Exception::class)
    fun i_click_current_repeat_medications() {
        recordSteps.clickCurrentRepeatMedications()
    }

    @Then("^I see current repeat medication information$")
    @Throws(Exception::class)
    fun i_see_current_repeat_medication_information() {
        Assert.assertTrue(recordSteps.isRepeatMedicationsAvailable())
    }

    @Then("^I see discontinued repeat medication information$")
    @Throws(Exception::class)
    fun i_see_discontinued_repeat_medication_information() {
        Assert.assertTrue(recordSteps.isDiscontinuedMedicationsAvailable())
    }

    @When("^I click discontinued repeat medications$")
    @Throws(Exception::class)
    fun i_click_discontinued_repeat_medications() {
        recordSteps.clickDiscontinuedRepeatMedications()
    }

    @Then("^I see a message indicating that I have no \"(.*)\" medications$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_medications(medication: String) {
        val msg = when (medication) {
            "acute" -> recordSteps.getNoAcuteMedicationMsg()
            "current repeat" -> recordSteps.getNoCurrentRepeatMedicationMsg()
            else -> recordSteps.getNoDiscontinuedRepeatMedicationMsg()
        }
        Assert.assertEquals("No information recorded", msg)
    }

    @Then("^I see a message indicating that I have no access to view my summary care record$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_my_record() {
        Assert.assertEquals("You don’t currently have online access to your medical record\nPlease contact your GP surgery for more information.", recordSteps.getSummaryCareNoAccessMessage())
    }

    @When("^I click the Immunisations section$")
    @Throws(Exception::class)
    fun i_click_the_Immunisaations_section() {
        recordSteps.clickImmunisations()
    }

    @Then("^I see heading Immunisations$")
    @Throws(Exception::class)
    fun i_see_heading_Immunisations() {
        Assert.assertEquals("Immunisations", recordSteps.getImmunisationsHeaderText())
    }

    @Then("^I see immunisation records displayed$")
    @Throws(Exception::class)
    fun i_see_immunisation_records_displayed() {
        Assert.assertEquals(2, recordSteps.getImmunisationRecordCount())
    }

    @When("^I click the Problems section$")
    @Throws(Exception::class)
    fun i_click_the_Problems_section() {
        recordSteps.clickProblems()
    }

    @Then("^I see heading Problems$")
    @Throws(Exception::class)
    fun i_see_heading_Problems() {
        Assert.assertEquals("Problems", recordSteps.getProblemsHeaderText())
    }

    @Then("^I see Problems records displayed$")
    @Throws(Exception::class)
    fun i_see_Problems_records_displayed() {
        Assert.assertEquals(3, recordSteps.getProblemsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view problems$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_problems() {
        Assert.assertEquals("You don't currently have access to this section", recordSteps.getProblemsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for problems$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_problems() {
        Assert.assertEquals("No information recorded", recordSteps.getProblemsMessage())
    }

    @Then("^I see an error occured message with problems$")
    @Throws(Exception::class)
    fun i_see_an_error_occured_message_for_problems() {
        Assert.assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getProblemsMessage())
    }

    @When("^I click the Consultations section$")
    @Throws(Exception::class)
    fun i_click_the_consultations_section() {
        recordSteps.clickConsultations()
    }

    @Then("^I see heading Consultations$")
    @Throws(Exception::class)
    fun i_see_heading_consultations() {
        Assert.assertEquals("Consultations", recordSteps.getConsultationsHeaderText())
    }

    @Then("^I see Consultations records displayed$")
    @Throws(Exception::class)
    fun i_see_consultations_records_displayed() {
        Assert.assertEquals(2, recordSteps.getConsultationsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view Consultations$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_consultations() {
        Assert.assertEquals("You don't currently have access to this section", recordSteps.getConsultationsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for Consultations$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_consultations() {
        Assert.assertEquals("No information recorded", recordSteps.getConsultationsMessage())
    }

    @Then("^I see an error occured message with Consultations$")
    @Throws(Exception::class)
    fun i_see_an_error_occured_message_for_consultations() {
        Assert.assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getConsultationsMessage())
    }

    @When("^I click the Events section$")
    @Throws(Exception::class)
    fun i_click_the_Events_section() {
        recordSteps.clickEvents()
    }

    @Then("^I see heading Events$")
    @Throws(Exception::class)
    fun i_see_heading_Events() {
        Assert.assertEquals("Consultations", recordSteps.getEventsHeaderText())
    }

    @Then("^I see Events records displayed$")
    @Throws(Exception::class)
    fun i_see_Events_records_displayed() {
        Assert.assertEquals(2, recordSteps.getEventsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view Events$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_Events() {
        Assert.assertEquals("You don't currently have access to this section", recordSteps.getEventsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for Events$")
    @Throws(Exception::class)
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_Events() {
        Assert.assertEquals("No information recorded", recordSteps.getEventsMessage())
    }

    @Then("^I see an error occured message with Events$")
    @Throws(Exception::class)
    fun i_see_an_error_occured_message_for_Events() {
        Assert.assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getEventsMessage())
    }

    @Then("^I see message No information recorded for this section$")
    @Throws(Exception::class)
    fun i_see_no_information_recorded_for_this_section_message() {
        Assert.assertEquals("No information recorded", recordSteps.getImmunisationsMessage())
    }

    @Then("^I see message You do not have access to this section$")
    @Throws(Exception::class)
    fun i_see_you_do_not_have_access_to_this_section_message() {
        Assert.assertEquals("You don't currently have access to this section", recordSteps.getImmunisationsMessage())
    }

    @Then("^I see message An error has occurred trying to retrieve this data$")
    @Throws(Exception::class)
    fun i_see_an_error_has_occurred_trying_to_retrieve_this_data() {
        Assert.assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getImmunisationsMessage())
    }

    @Then("^the field indicating supplier is set to (.*)$")
    fun thenTheFlagIndicatingSupplierIsSetTo(supplier: String) {
        var result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(supplier, result.response.supplier)
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        Assert.assertEquals(count, recordSteps.getTestResultCount())
    }
}
