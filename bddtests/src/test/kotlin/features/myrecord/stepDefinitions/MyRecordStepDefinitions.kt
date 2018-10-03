package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.MedicationsData
import mocking.data.myrecord.ProblemsData
import mocking.data.myrecord.TestResultsData
import mocking.data.myrecord.TppDcrData
import mocking.data.myrecord.ViewPatientOverviewData
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.tpp.models.Error
import mocking.vision.VisionConstants
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordWarningPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.time.OffsetDateTime

open class MyRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps

    lateinit var myRecordWarningPage: MyRecordWarningPage

    lateinit var myRecordInfoPage: MyRecordInfoPage

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
            "VISION" -> {
                mockingClient.forVision {
                    allergiesRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion)
                    ).respondWithSuccess(AllergiesData.getVisionAllergiesData(0))

                }
            }
        }
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

    @Given("^there is an unknown error getting allergies for (.*)$")
    fun thereIsAnUnknownErrorGettingAllergiesFor(service: String) {
        setPatientToDefaultFor(service)
        when (service) {
            "VISION" -> {
                mockingClient.forVision {
                    allergiesRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion)
                    ).respondWithUnknownError()
                }
            }
        }
    }

    @When("I click a test result$")
    fun i_click_a_test_result() {
        myRecordInfoPage.testResults.clickFirst()
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
        i_see_header_text_is_My_medical_record()
        i_see_agree_and_continue_button()
        i_see_back_to_home_button()
    }

    @Then("^I see the my record warning page")
    fun iSeeTheMyRecordWarningPage() {
        i_see_record_warning_page_opened()
        i_see_header_text_is_My_medical_record()
        i_see_your_record_may_contain_sensitive_information_message()
        i_see_list_of_sensitive_data_information()
        i_see_agree_and_continue_button()
        i_see_back_to_home_button()
        i_see_my_record_button_on_the_nav_bar_is_highlighted()

    }

    @Then("^I see the my medical record page$")
    fun iSeeTheMyMedicalRecordPage() {
        i_see_header_text_is_My_medical_record()
        iSeeTheHeadingOnMyRecord("My details")
        iSeePatientInformationDetails()
        i_see_my_record_button_on_the_nav_bar_is_highlighted()
    }


    @Then("^I see header text is My medical record$")
    fun i_see_header_text_is_My_medical_record() {
        myRecordWarningPage.waitForPageHeaderText("My medical record")
    }

    @Then("^I see your record may contain sensitive information message$")
    fun i_see_your_record_may_contain_sensitive_information_message() {
        assertEquals("Your record may contain sensitive information", myRecordWarningPage.warningText())
    }

    @Then("^I see list of sensitive data information$")
    fun i_see_list_of_sensitive_data_information() {
        val expected = ArrayList<String>()
        expected.add("personal data, such as your details, allergies and medications")
        expected.add("clinical terms that you may not be familiar with")
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
        assertArrayEquals(expected.toArray(), myRecordWarningPage.getSensitiveList().toArray())
    }

    @Then("^I see agree and continue button$")
    fun i_see_agree_and_continue_button() {
        Assert.assertTrue("isAgreePresent", myRecordWarningPage.isAgreePresent())
    }

    @Then("^I see back to home button$")
    fun i_see_back_to_home_button() {
        Assert.assertTrue("isBackToHomePresent", myRecordWarningPage.isBackToHomePresent())
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
        myRecordWarningPage.clickAgreeAndContinue()
    }

    @Then("^the my record information screen is loaded$")
    fun the_my_record_information_screen_is_loaded() {
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
    }

    @When("^I click the back to home button$")
    fun i_click_the_back_to_home_button() {
        myRecordWarningPage.clickBacktoHome()
    }

    @Then("^I will return to the home page$")
    fun i_will_return_to_the_home_page() {
        navHeader.assertHomePageHeaderVisible()
    }

    @Then("^No navigation menu bar item will be selected$")
    fun no_navigation_menu_bar_item_will_be_selected() {
        assertTrue(!nav.hasSelectedTab("SYMPTOMS"))
        assertTrue(!nav.hasSelectedTab("APPOINTMENTS"))
        assertTrue(!nav.hasSelectedTab("PRESCRIPTIONS"))
        assertTrue(!nav.hasSelectedTab("MY_RECORD"))
        assertTrue(!nav.hasSelectedTab("MORE"))
    }

    @Then("^I see the patient information details$")
    fun iSeePatientInformationDetails() {
        val sex = this.patient.sex.name;
        val address = "${this.patient.address.houseNameFlatNumber}, ${this.patient.address.numberStreet}, ${this.patient.address.village}, ${this.patient.address.town}, ${this.patient.address.county}, ${this.patient.address.postcode}";

        myRecordInfoPage.assertLabelAndValue("Name", patient.formattedFullName())
        myRecordInfoPage.assertLabelAndValue("Date of birth", patient.formattedDateOfBirth())
        myRecordInfoPage.assertLabelAndValue("Sex", sex)
        myRecordInfoPage.assertLabelAndValue("Address", address)
        myRecordInfoPage.assertLabelAndValue("NHS number", patient.formattedNHSNumber())
    }

    @Given("^I am on my record information page$")
    fun i_am_on_my_record_information_page() {
        browser.goToApp()
        login.using(this.patient)
        nav.select("MY_RECORD")
        myRecordWarningPage.clickAgreeAndContinue()
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
    }

    @Then("^I can see the clinical abbreviations link$")
    fun i_can_see_the_clinical_abbreviations_link() {
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
    }

    @Then("^I click the clinical abbreviations link$")
    fun i_click_the_clinical_abbreviations_link() {
        myRecordInfoPage.clickClinicalAbbreviationsLink()
    }

    @When("^I click My details heading$")
    fun i_click_My_details_heading() {
        myRecordInfoPage.myDetails.toggleShrub()
    }

    @Then("^I do not see patient information details$")
    fun i_do_not_see_patient_information_details() {
        assertFalse("Name field was visible.", myRecordInfoPage.isNameVisible())
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

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun i_see_Service_not_offered_by_GP_or_to_specific_user_or_access_revoked_warning_message() {
        assertEquals("You do not currently have online access to your medical record\nContact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    fun i_see_one_or_more_drug_type_allergies_record_displayed() {
        assertEquals(2, myRecordInfoPage.allergies.allRecordItems().count())
        val expected = ArrayList<String>()
        for (i in 1..2) {
            expected.add(AllergiesData.TERM)
        }

        assertArrayEquals(expected.toArray(), myRecordInfoPage.allergies.allRecordItemBodies()
                .toTypedArray())
    }

    @Then("^I see 5 allergies with different date formats$")
    fun i_see_five_allergies_with_different_date_formats() {

        assertEquals(5, myRecordInfoPage.allergies.allRecordItems().count())
        val dates = myRecordInfoPage.allergies.allRecordItemLabels()

        assertContains(dates, "15 May 2018")
        assertContains(dates, "15 May 2018")
        assertContains(dates, "May 2018")
        assertContains(dates, "2018")
        assertContains(dates, "15 May 2018 09:52")
    }

    private fun assertContains(actualDates: List<String>, expected: String) {

        assertTrue("Expected to contain $expected, but was ${actualDates.joinToString()}",
                actualDates.contains(expected))
    }

    @Then("^I see acute medication information$")
    fun i_see_acute_medication_information() {
        assertTrue(myRecordInfoPage.acuteMedications.firstElement.isVisible)
    }

    @When("^I click the test result section$")
    fun i_click_the_test_result_section() {
        myRecordInfoPage.testResults.toggleShrub()
    }

    @Then("^I see one test result with one value$")
    fun i_see_one_test_result_with_one_value() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with one value and a range$")
    fun i_see_one_test_result_with_one_value_and_a_range() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with multiple child values$")
    fun i_see_one_test_result_with_multiple_child_values() {
        assertTrue("Expected test result equal to or less than 1, but was" +
                "${myRecordInfoPage.testResults.allRecordItems().size}",
                myRecordInfoPage.testResults.allRecordItems().size >= 1)
        assertTrue("Expected child test result equal to or less than 1, but was " +
                "${myRecordInfoPage.getTestResultChildCount()}",
                myRecordInfoPage.getTestResultChildCount() > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges$")
    fun i_see_test_results_with_multiple_child_values_some_of_which_ave_ranges() {
        assertTrue("Expected test result equal to or greater than 1, but was" +
                "${myRecordInfoPage.testResults.allRecordItems().size}"
                , myRecordInfoPage.testResults.allRecordItems().size >= 1)
        assertTrue("Expected child test result equal to or greater than 1, but was " +
                "${myRecordInfoPage.getTestResultChildCount()}",
                myRecordInfoPage.getTestResultChildCount() > 1)
    }

    @Then("^I see the (.*) heading on My Record$")
    fun iSeeTheHeadingOnMyRecord(heading: String) {
        myRecordInfoPage.assertSectionHeaderIsVisible(heading)
    }

    @When("^I click the (.*) section on My Record$")
    fun iClickOnTheSection(heading: String) {
        myRecordInfoPage.getSection(heading).toggleShrub()
    }

    @Then("^I see the (.*) section collapsed on My Record$")
    fun iSeeTheSectionCollapsed(heading: String) {
        val section = myRecordInfoPage.getSection(heading)
        assertFalse("Expected section paragraph to not be visible", section.firstParagraph.isCurrentlyVisible)
    }

    @Then("^I see the test result heading for (.*)$")
    fun i_see_the_test_result_heading(getService: String) {
        val header = when (getService) {
            "TPP" -> {
                "Test results (past 6 months)"
            }
            else -> {
                "Test results"
            }
        }
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @Then("^I see the test result section collapsed$")
    fun i_see_the_test_result_section_collapsed() {
        assertFalse(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see test result information$")
    fun i_see_test_result_information() {
        assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see current repeat medication information$")
    fun i_see_current_repeat_medication_information() {
        assertTrue(myRecordInfoPage.repeatMedications.firstElement.isVisible)
    }

    @Then("^I see discontinued repeat medication information$")
    fun i_see_discontinued_repeat_medication_information() {
        assertTrue(myRecordInfoPage.discontinuedRepeatMedications.firstElement.isVisible)
    }

    @Then("^I see a message indicating that I have no access to view my summary care record$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_my_record() {
        assertEquals("You do not currently have online access to your medical record\n" + "Contact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see immunisation records displayed$")
    fun i_see_immunisation_records_displayed() {
        assertEquals(2, myRecordInfoPage.immunisations.allRecordItems().count())
    }

    @Then("^I see Problems records displayed$")
    fun i_see_Problems_records_displayed() {
        assertEquals(3, myRecordInfoPage.problems.allRecordItems().count())
    }

    @Then("^I see Consultations records displayed$")
    fun i_see_consultations_records_displayed() {
        assertEquals(2, myRecordInfoPage.consultations.allRecordItems().count())
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        assertEquals("Expected test results", count, myRecordInfoPage.testResults.allRecordItems().count())
    }

    @Then("^I see a message indicating that I have no access to view (.*) on My Record$")
    fun i_see_a_message_indicating_that_I_have_no_access_to_view_section(heading: String) {
        assertTextInSection(heading, "You do not currently have access to this section")
    }

    @Then("^I see an error occurred message with (.*) on My Record$")
    fun i_see_an_error_occured_message_for_problems(heading: String) {
        assertTextInSection(heading, "An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message indicating that I have no information recorded for (.*) on My Record$")
    fun i_see_a_message_indicating_that_I_have_No_information_recorded(heading: String) {
        assertTextInSection(heading, "No information recorded")
    }

    private fun assertTextInSection(heading: String, message: String) {
        val section = myRecordInfoPage.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }

    @Then("^the field indicating supplier is set to (.*)$")
    fun thenTheFlagIndicatingSupplierIsSetTo(supplier: String) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(supplier, result.response.supplier)
    }

    @Then("^I see a drug and non drug allergy record from VISION$")
    fun i_see_a_drug_and_non_drug_allergy_record_from_vision() {
        val allergyMessages = myRecordInfoPage.allergies.allRecordItemBodies()
        val expectedMessages = listOf(
                "H/O: drug allergy",
                "Paracetamol 500mg capsules",
                "Leg swelling",
                "Pollen"
        )
        assertTrue("Expected records",allergyMessages.size == expectedMessages.size)
        allergyMessages.forEachIndexed { i, message -> assertTrue(message == expectedMessages[i]) }
    }
}
