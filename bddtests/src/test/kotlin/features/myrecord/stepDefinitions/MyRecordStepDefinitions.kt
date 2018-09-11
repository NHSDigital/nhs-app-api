package features.myrecord.stepDefinitions

import config.Config
import constants.AppointmentDateTimeFormat
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.myrecord.steps.MyRecordSteps
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.data.myrecord.*
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import mocking.tpp.models.Error
import org.junit.Assert.*
import java.time.OffsetDateTime
import utils.DateConverter

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
    lateinit var navHeader: NavHeaderSteps


    @Given("^the my record wiremocks are initialised for (.*)$")
    fun givenMyRecordWiremocksAreInitialisedfor(getService: String) {
        setPatientToDefaultFor(getService)
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)
        SessionCreateJourneyFactory.getForSupplier(getService, mockingClient).createFor(this.patient)

        when (getService) {
            "EMIS" -> {
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
    fun i_click_a_test_result() {
        recordSteps.clickTestResult()
    }

    @When("^I click my record button on menu bar$")
    fun i_click_my_record_button_on_menu_bar() {
        nav.select("MY_RECORD")
    }

    @When("^I enter url address for my record directly into the url$")
    fun i_enter_url_address_for_my_record_directly_into_the_url() {
        val fullUrl = Config.instance.url + "/my-record"
        browser.browseTo(fullUrl)
    }

    @Then("^I see record warning page opened$")
    fun i_see_record_warning_page_opened() {
        assertTrue(recordSteps.isAgreePresent())
    }

    @Then("^I see header text is My medical record$")
    fun i_see_header_text_is_My_medical_record() {
        recordSteps.waitForCorrectHeader()
    }

    @Then("^I see your record may contain sensitive information message$")
    fun i_see_your_record_may_contain_sensitive_information_message() {
        assertEquals("Your record may contain sensitive information", recordSteps.warningText())
    }

    @Then("^I see list of sensitive data information$")
    fun i_see_list_of_sensitive_data_information() {
        val expected = ArrayList<String>()
        expected.add("personal data, such as your details, allergies and medications")
        expected.add("clinical terms that you may not be familiar with")
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
        assertArrayEquals(expected.toArray(), recordSteps.getSensitiveList().toArray())
    }

    @Then("^I see agree and continue button$")
    fun i_see_agree_and_continue_button() {
        assertTrue(recordSteps.isAgreePresent())
    }

    @Then("^I see back to home button$")
    fun i_see_back_to_home_button() {
        assertTrue(recordSteps.isBackToHomePresent())
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    fun i_see_my_record_button_on_the_nav_bar_is_highlighted() {
        assertTrue(nav.hasSelectedTab("MY_RECORD"))
    }

    @Given("^I am on the record warning page$")
    fun i_am_on_the_record_warning_page() {
        browser.goToApp()
        login.using(this.patient)
        nav.select("MY_RECORD")
    }

    @When("^I click agree and continue$")
    fun i_click_agree_and_continue() {
        recordSteps.clickAgreeandContinue()
    }

    @Then("^the my record information screen is loaded$")
    fun the_my_record_information_screen_is_loaded() {
        assertTrue(recordSteps.isOnMyRecordInfoPage())
    }

    @When("^I click the back to home button$")
    fun i_click_the_back_to_home_button() {
        recordSteps.clickBacktoHome()
    }

    @Then("^I will return to the home page$")
    fun i_will_return_to_the_home_page() {
        navHeader.assertHeaderVisible()
    }

    @Then("^No navigation menu bar item will be selected$")
    fun no_navigation_menu_bar_item_will_be_selected() {
        assertTrue(!nav.hasSelectedTab("SYMPTOMS"))
        assertTrue(!nav.hasSelectedTab("APPOINTMENTS"))
        assertTrue(!nav.hasSelectedTab("PRESCRIPTIONS"))
        assertTrue(!nav.hasSelectedTab("MY_RECORD"))
        assertTrue(!nav.hasSelectedTab("MORE"))
    }

    @Then("^I see heading My details$")
    fun i_see_heading_My_details() {
        assertEquals("My details", recordSteps.getMyDetailsLabelText())
    }

    @Then("^I see the patient information details$")
    @Throws(Exception::class)
    fun i_see_the_patient_information_details() {

        // See the labels
        assertEquals("Name", recordSteps.getNameLabelText())
        assertEquals("Date of birth", recordSteps.getDOBLabelText())
        assertEquals("Sex", recordSteps.getSexLabelText())
        assertEquals("Address", recordSteps.getAddressLabelText())
        assertEquals("NHS number", recordSteps.getNHSNumberLabelText())

        // populated with correct information
        val name = "${this.patient.title} ${this.patient.firstName} ${this.patient.surname}";
        val dob = DateConverter.ConvertDateToDateTimeFormat(this.patient.dateOfBirth, AppointmentDateTimeFormat.mockDataDobFormat, AppointmentDateTimeFormat.frontendDobDateFormat)
        val sex = this.patient.sex.name;
        val address = "${this.patient.address.houseNameFlatNumber}, ${this.patient.address.numberStreet}, ${this.patient.address.village}, ${this.patient.address.town}, ${this.patient.address.county}, ${this.patient.address.postcode}";
        val nhsNumbers = StringBuilder(this.patient.nhsNumbers.first()).insert(6," ").insert(3, " ").toString();

        assertEquals(name, recordSteps.getNameControlText())
        assertEquals(dob, recordSteps.getDOBText())
        assertEquals(sex, recordSteps.getSexText())
        assertEquals(address, recordSteps.getAddressText())
        assertEquals(nhsNumbers, recordSteps.getNHSNumberText())
    }

    @Given("^I am on my record information page$")
    fun i_am_on_my_record_information_page() {
        browser.goToApp()
        login.using(this.patient)
        nav.select("MY_RECORD")
        recordSteps.clickAgreeandContinue()
        assertTrue(recordSteps.isOnMyRecordInfoPage())
        assertTrue(recordSteps.canSeeClinicalAbbreviationsLink())
    }

    @Then("^I can see the clinical abbreviations link$")
    fun i_can_see_the_clinical_abbreviations_link() {
        assertTrue(recordSteps.canSeeClinicalAbbreviationsLink())
    }

    @Then("^I click the clinical abbreviations link$")
    fun i_click_the_clinical_abbreviations_link() {
        recordSteps.clickClinicalAbbreviationsLink()
    }

    @When("^I click My details heading$")
    fun i_click_My_details_heading() {
        recordSteps.clickMyDetails()
    }

    @Then("^I do not see patient information details$")
    fun i_do_not_see_patient_information_details() {
        assertFalse("Name field was visible.", recordSteps.isNameVisible())
    }

    @When("^I get the users my record data$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord()

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
    fun i_see_the_Allergies_and_Adverse_Reactions_heading() {
        assertEquals("Allergies and adverse reactions", recordSteps.getAllergiesAndAdverseReactionsHeaderText())
    }

    @Then("^I see the Allergies and Adverse Reactions section collapsed$")
    fun i_see_the_Allergies_and_Adverse_Reactions_section_collapsed() {
        assertFalse(recordSteps.isAllergiesTextMsgVisible())
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun  i_see_Service_not_offered_by_GP_or_to_specific_user_or_access_revoked_warning_message() {
        assertEquals("You do not currently have online access to your medical record\nContact your GP surgery for more information.", recordSteps.getSummaryCareNoAccessMessage())
    }

    @Then("^I see a message indicating I have no allergies$")
    fun i_see_a_message_indicating_I_have_no_allergies() {
        assertEquals("No information recorded", recordSteps.getNoAllergyMessage())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    fun i_see_one_or_more_drug_type_allergies_record_displayed() {
        assertEquals(2, recordSteps.getAllergyCount())
        val expected = ArrayList<String>()
        for (i in 1..2) {
            expected.add(AllergiesData.TERM)
        }

        assertArrayEquals(expected.toArray(), recordSteps.getAllergyMessages().toArray())
    }

    @Then("^I see 5 allergies with different date formats$")
    fun i_see_five_allergies_with_different_date_formats() {

        assertEquals(5, recordSteps.getAllergyCount())
        assertTrue(recordSteps.getAllergyDates().contains("15 May 2018"))
        assertTrue(recordSteps.getAllergyDates().contains("May 2018"))
        assertTrue(recordSteps.getAllergyDates().contains("2018"))
        assertTrue(recordSteps.getAllergyDates().contains("15 May 2018 09:52"))
    }

    @Then("^I see one or more non drug type allergies record displayed$")
    fun i_see_one_or_more_non_drug_type_allergies_record_displayed() {
        assertEquals("non Drug Allergy", recordSteps.getAllergyMessage())
    }

    @Given("^I see heading Acute medications$")
    fun i_see_heading_acute_medications() {
        assertEquals("Acute (short-term) medications", recordSteps.getAcuteMedicationsHeaderText())
    }

    @When("^I click acute medications$")
    fun i_click_acute_medications() {
        recordSteps.clickAcuteMedications()
    }

    @Then("^I see acute medication information$")
    fun i_see_acute_medication_information() {
        assertTrue(recordSteps.isAcuteMedicationsAvailable())
    }

    @When("^I click the test result section$")
    fun i_click_the_test_result_section() {
        recordSteps.clickTestResultsSection()
    }

    @Then("^I see one test result with one value$")
    fun i_see_one_test_result_with_one_value() {
        assertEquals(1, recordSteps.getTestResultCount())
        assertEquals(1, recordSteps.getTestResultChildCount())
        //Assert.assertEquals("No information recorded for this section", recordSteps.getTestResultMsg())
    }

    @Then("^I see one test result with one value and a range$")
    fun i_see_one_test_result_with_one_value_and_a_range() {
        assertEquals(1, recordSteps.getTestResultCount())
        assertEquals(1, recordSteps.getTestResultChildCount())
    }

    @Then("^I see one test result with multiple child values$")
    fun i_see_one_test_result_with_multiple_child_values() {
        assertTrue(recordSteps.getTestResultCount() >= 1)
        assertTrue(recordSteps.getTestResultChildCount() > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges$")
    fun i_see_test_results_with_multiple_child_values_some_of_which_ave_ranges() {
        assertTrue(recordSteps.getTestResultCount() >= 1)
        assertTrue(recordSteps.getTestResultChildCount() > 1)
    }

    @Then("^I see the test result heading for (.*)$")
    fun i_see_the_test_result_heading(getService: String) {
        when (getService) {
            "TPP" -> {
                assertEquals("Test results (past 6 months)", recordSteps.getTestResultsHeaderText())
            }
            else -> {
                assertEquals("Test results", recordSteps.getTestResultsHeaderText())
            }
        }
    }

    @Then("^I see the test result section collapsed$")
    fun i_see_the_test_result_section_collapsed() {
        assertFalse(recordSteps.isTestResultsTextMsgVisible())
    }

    @Then("^I see test result information$")
    fun i_see_test_result_information() {
        assertTrue(recordSteps.isTestResultsTextMsgVisible())
    }

    @Then("^I see a message indicating that I have no access to view test result$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_test_result() {
        assertEquals("You do not currently have access to this section", recordSteps.getTestResultsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for this section$")
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_this_section() {
        assertEquals("No information recorded", recordSteps.getTestResultsMessage())
    }

    @Then("^I see an error occured message$")
    fun i_see_an_error_occured_message() {
        assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getTestResultsMessage())
    }

    @Given("^I see heading Current repeat medications$")
    fun i_see_heading_Current_repeat_medications() {

    }

    @When("^I click current repeat medications$")
    fun i_click_current_repeat_medications() {
        recordSteps.clickCurrentRepeatMedications()
    }

    @Then("^I see current repeat medication information$")
    fun i_see_current_repeat_medication_information() {
        assertTrue(recordSteps.isRepeatMedicationsAvailable())
    }

    @Then("^I see discontinued repeat medication information$")
    fun i_see_discontinued_repeat_medication_information() {
        assertTrue(recordSteps.isDiscontinuedMedicationsAvailable())
    }

    @When("^I click discontinued repeat medications$")
    fun i_click_discontinued_repeat_medications() {
        recordSteps.clickDiscontinuedRepeatMedications()
    }

    @Then("^I see a message indicating that I have no \"(.*)\" medications$")
    fun i_see_a_message_indicating_that_I_have_no_medications(medication: String) {
        val msg = when (medication) {
            "acute" -> recordSteps.getNoAcuteMedicationMsg()
            "current repeat" -> recordSteps.getNoCurrentRepeatMedicationMsg()
            else -> recordSteps.getNoDiscontinuedRepeatMedicationMsg()
        }
        assertEquals("No information recorded", msg)
    }

    @Then("^I see a message indicating that I have no access to view my summary care record$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_my_record() {
        assertEquals("You do not currently have online access to your medical record\n" + "Contact your GP surgery for more information.", recordSteps.getSummaryCareNoAccessMessage())
    }

    @When("^I click the Immunisations section$")
    fun i_click_the_Immunisaations_section() {
        recordSteps.clickImmunisations()
    }

    @Then("^I see heading Immunisations$")
    fun i_see_heading_Immunisations() {
        assertEquals("Immunisations", recordSteps.getImmunisationsHeaderText())
    }

    @Then("^I see immunisation records displayed$")
    fun i_see_immunisation_records_displayed() {
        assertEquals(2, recordSteps.getImmunisationRecordCount())
    }

    @When("^I click the Problems section$")
    fun i_click_the_Problems_section() {
        recordSteps.clickProblems()
    }

    @Then("^I see heading Problems$")
    fun i_see_heading_Problems() {
        assertEquals("Problems", recordSteps.getProblemsHeaderText())
    }

    @Then("^I see Problems records displayed$")
    fun i_see_Problems_records_displayed() {
        assertEquals(3, recordSteps.getProblemsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view problems$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_problems() {
        assertEquals("You do not currently have access to this section", recordSteps.getProblemsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for problems$")
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_problems() {
        assertEquals("No information recorded", recordSteps.getProblemsMessage())
    }

    @Then("^I see an error occured message with problems$")
    fun i_see_an_error_occured_message_for_problems() {
        assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getProblemsMessage())
    }

    @When("^I click the Consultations section EMIS$")
    fun i_click_the_consultations_section_EMIS() {
        recordSteps.clickConsultations()
    }

    @Then("^I see heading Consultations EMIS$")
    fun i_see_heading_consultations_EMIS() {
        assertEquals("Consultations", recordSteps.getConsultationsHeaderText())
    }

    @Then("^I see Consultations records displayed EMIS$")
    fun i_see_consultations_records_displayed_EMIS() {
        assertEquals(2, recordSteps.getConsultationsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view Consultations EMIS$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_consultations_EMIS() {
        assertEquals("You do not currently have access to this section", recordSteps.getConsultationsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for Consultations EMIS$")
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_consultations_EMIS() {
        assertEquals("No information recorded", recordSteps.getConsultationsMessage())
    }

    @Then("^I see an error occurred message with Consultations EMIS$")
    fun i_see_an_error_occurred_message_for_consultations_EMIS() {
        assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getConsultationsMessage())
    }

    @When("^I click the Consultations section TPP$")
    fun i_click_the_Consultations_section_TPP() {
        recordSteps.clickEvents()
    }

    @Then("^I see heading Consultations TPP$")
    fun i_see_heading_consultations_TPP() {
        assertEquals("Consultations", recordSteps.getEventsHeaderText())
    }

    @Then("^I see Consultations records displayed TPP$")
    fun i_see_consultations_records_displayed_TPP() {
        assertEquals(2, recordSteps.getEventsRecordCount())
    }

    @Then("^I see a message indicating that I have no access to view Consultations TPP$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_Consultations_TPP() {
        assertEquals("You do not currently have access to this section", recordSteps.getEventsMessage())
    }

    @Then("^I see a message indicating that I have no information recorded for Consultations TPP$")
    fun i_see_a_message_indicating_that_I_have_No_information_recorded_for_Consultations_TPP() {
        assertEquals("No information recorded", recordSteps.getEventsMessage())
    }

    @Then("^I see an error occurred message with Consultations TPP$")
    fun i_see_an_error_occurred_message_for_Consultations_TPP() {
        assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getEventsMessage())
    }

    @Then("^I see message No information recorded for this section$")
    fun i_see_no_information_recorded_for_this_section_message() {
        assertEquals("No information recorded", recordSteps.getImmunisationsMessage())
    }

    @Then("^I see message You do not have access to this section$")
    fun i_see_you_do_not_have_access_to_this_section_message() {
        assertEquals("You do not currently have access to this section", recordSteps.getImmunisationsMessage())
    }

    @Then("^I see message An error has occurred trying to retrieve this data$")
    fun i_see_an_error_has_occurred_trying_to_retrieve_this_data() {
        assertEquals("An error has occurred trying to retrieve this data.", recordSteps.getImmunisationsMessage())
    }

    @Then("^the field indicating supplier is set to (.*)$")
    fun thenTheFlagIndicatingSupplierIsSetTo(supplier: String) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(supplier, result.response.supplier)
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        assertEquals(count, recordSteps.getTestResultCount())
    }
}
