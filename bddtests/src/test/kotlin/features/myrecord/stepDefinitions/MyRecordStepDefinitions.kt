package features.myrecord.stepDefinitions

import config.Config
import constants.ErrorResponseCodeTpp
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
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.tpp.models.Error
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.allergiesView
import mocking.vision.VisionConstants.htmlResponseFormat
import mocking.vision.VisionConstants.immunisationsView
import mocking.vision.VisionConstants.medicationsView
import mocking.vision.VisionConstants.xmlResponseFormat
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
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.time.OffsetDateTime

private const val NUMBER_OF_PRESCRIPTIONS = 5
private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
private const val END_DATE = 60L

open class MyRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps

    lateinit var headerNative: HeaderNative

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
                    myRecord.testResultsRequest(patient)
                            .respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
                }

                mockingClient.forEmis {
                    myRecord.immunisationsRequest(patient)
                            .respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
                }

                mockingClient.forEmis {
                    myRecord.allergiesRequest(patient).respondWithSuccess(AllergiesData.getEmisDefaultAllergyModel())
                }

                mockingClient.forEmis {
                    myRecord.medicationsRequest(patient)
                            .respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
                }

                mockingClient.forEmis {
                    myRecord.problemsRequest(patient).respondWithSuccess(ProblemsData.getDefaultProblemModel())
                }

                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis)
                            .respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithSuccess(ViewPatientOverviewData.getTppViewPatientOverviewData())
                }

                mockingClient.forTpp {
                    myRecord.patientRecordRequest(patient.tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getDefaultTppDcrData())
                }

                val startDate = OffsetDateTime.now()
                val endDate = startDate.minusDays(END_DATE)

                mockingClient.forTpp {
                    myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                            .respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                            view = allergiesView,
                            responseFormat = htmlResponseFormat
                    ).respondWithSuccess(AllergiesData.getVisionAllergiesData(0))

                }

                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                            view = immunisationsView,
                            responseFormat = xmlResponseFormat
                    ).respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())

                }

                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                            view = medicationsView,
                            responseFormat = xmlResponseFormat
                    ).respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())

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
                    myRecord.allergiesRequest(patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "Requested record access is disabled by the practice",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                            view = allergiesView,
                            responseFormat = "HTML"
                    ).respondWithAccessDeniedError()
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
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                            view = allergiesView,
                            responseFormat = htmlResponseFormat
                    ).respondWithUnknownError()
                }
            }
        }
    }

    @When("I click a test result$")
    fun whenIClickATestResult() {
        myRecordInfoPage.testResults.clickFirst()
    }

    @When("^I click my record button on menu bar$")
    fun whenIClickMyRecordButtonOnMenuBar() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @When("^I enter url address for my record directly into the url$")
    fun whenIEnterUrlAddressForMyRecordDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record"
        browser.browseTo(fullUrl)
    }

    @Then("^I see record warning page opened$")
    fun thenISeeRecordWarningPageOpened() {
        thenISeeHeaderTextIsMyMedicalRecord()
        thenISeeAgreeAndContinueButton()
        thenISeeBackToHomeButton()
    }

    @Then("^I see the my record warning page")
    fun iSeeTheMyRecordWarningPage() {
        thenISeeRecordWarningPageOpened()
        thenISeeHeaderTextIsMyMedicalRecord()
        theISeeYourRecordMayContainSensitiveInformationMessage()
        thenISeeListOfSensitiveDataInformation()
        thenISeeAgreeAndContinueButton()
        thenISeeBackToHomeButton()
        thenISeeMyRecordButtonOnTheNavBarIsHighlighted()

    }

    @Then("^I see the my medical record page$")
    fun iSeeTheMyMedicalRecordPage() {
        thenISeeHeaderTextIsMyMedicalRecord()
        iSeeTheHeadingOnMyRecord("My details")
        iSeePatientInformationDetails()
        thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }


    @Then("^I see header text is My medical record$")
    fun thenISeeHeaderTextIsMyMedicalRecord() {
        headerNative.waitForPageHeaderText("My medical record")
    }

    @Then("^I see your record may contain sensitive information message$")
    fun theISeeYourRecordMayContainSensitiveInformationMessage() {
        assertEquals("Your record may contain sensitive information", myRecordWarningPage.warningText())
    }

    @Then("^I see list of sensitive data information$")
    fun thenISeeListOfSensitiveDataInformation() {
        val expected = ArrayList<String>()
        expected.add("personal data, such as your details, allergies and medications")
        expected.add("clinical terms that you may not be familiar with")
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
        assertArrayEquals(expected.toArray(), myRecordWarningPage.getSensitiveList().toArray())
    }

    @Then("^I see agree and continue button$")
    fun thenISeeAgreeAndContinueButton() {
        Assert.assertTrue("isAgreePresent", myRecordWarningPage.isAgreePresent())
    }

    @Then("^I see back to home button$")
    fun thenISeeBackToHomeButton() {
        Assert.assertTrue("isBackToHomePresent", myRecordWarningPage.isBackToHomePresent())
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    fun thenISeeMyRecordButtonOnTheNavBarIsHighlighted() {
        assertTrue(nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
    }

    @Given("^I am on the record warning page$")
    fun givenIAmOnTheRecordWarningPage() {
        browser.goToApp()
        login.using(this.patient)
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @When("^I click agree and continue$")
    fun whenIClickAgreeAndContinue() {
        myRecordWarningPage.clickAgreeAndContinue()
    }

    @Then("^the my record information screen is loaded$")
    fun thenTheMyRecordInformationScreenIsLoaded() {
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
    }

    @When("^I click the back to home button$")
    fun whenIClickTheBackToHomeButton() {
        myRecordWarningPage.clickBacktoHome()
    }

    @Then("^I will return to the home page$")
    fun thenIWillReturnToTheHomePage() {
        navHeader.assertHomePageHeaderVisible()
    }

    @Then("^No navigation menu bar item will be selected$")
    fun thenNoNavigationMenuBarItemWillBeSelected() {
        assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.SYMPTOMS))
        assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.APPOINTMENTS))
        assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.PRESCRIPTIONS))
        assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
        assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MORE))
    }

    @Then("^I see the patient information details$")
    fun iSeePatientInformationDetails() {
        val sex = this.patient.sex.name;
        val address = "${this.patient.address.houseNameFlatNumber}, " +
                "${this.patient.address.numberStreet}, " +
                "${this.patient.address.village}, " +
                "${this.patient.address.town}, " +
                "${this.patient.address.county}, " +
                "${this.patient.address.postcode}"

        myRecordInfoPage.assertLabelAndValue("Name", patient.formattedFullName())
        myRecordInfoPage.assertLabelAndValue("Date of birth", patient.formattedDateOfBirth())
        myRecordInfoPage.assertLabelAndValue("Sex", sex)
        myRecordInfoPage.assertLabelAndValue("Address", address)
        myRecordInfoPage.assertLabelAndValue("NHS number", patient.formattedNHSNumber())
    }

    @Given("^I am on my record information page$")
    fun givenIAmOnMyRecordInformationPage() {
        browser.goToApp()
        login.using(this.patient)
        nav.select(NavBarNative.NavBarType.MY_RECORD)
        myRecordWarningPage.clickAgreeAndContinue()
        myRecordInfoPage.waitForNativeStepToComplete()
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
        myRecordInfoPage.waitForSpinnerToDisappear()
    }

    @Then("^I can see the clinical abbreviations link$")
    fun thenICanSeeTheClinicalAbbreviationsLink() {
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
    }

    @Then("^I click the clinical abbreviations link$")
    fun thenIClickTheClinicalAbbreviationsLink() {
        myRecordInfoPage.clickClinicalAbbreviationsLink()
    }

    @When("^I click My details heading$")
    fun whenIClickMyDetailsHeading() {
        myRecordInfoPage.myDetails.toggleShrub()
    }

    @Then("^I do not see patient information details$")
    fun thenIDoNotSeePatientInformationDetails() {
        assertFalse("Name field was visible.", myRecordInfoPage.isNameVisible())
    }

    @When("^I get the users my record data$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun thenISeeServiceNotOfferedByGPOrToSpecificUserOrAccessRevokedWarningMessage() {
        assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see one or more drug type allergies record displayed$")
    fun thenISeeOneOrMoreDrugTypeAllergiesRecordDisplayed() {
        assertEquals(2, myRecordInfoPage.allergies.allRecordItems().count())
        val expected = ArrayList<String>()
        for (i in 1..2) {
            expected.add(AllergiesData.TERM)
        }

        assertArrayEquals(expected.toArray(), myRecordInfoPage.allergies.allRecordItemBodies()
                .toTypedArray())
    }

    @Then("^I see 5 allergies with different date formats$")
    fun thenISeeFiveAllergiesWithDifferentDateFormats() {

        assertEquals(NUMBER_OF_PRESCRIPTIONS, myRecordInfoPage.allergies.allRecordItems().count())
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
    fun thenISeeAcuteMedicationInformation() {
        myRecordInfoPage.acuteMedications.firstElement.assertIsVisible()
    }

    @When("^I click the test result section$")
    fun whenIClickTheTestResultSection() {
        myRecordInfoPage.testResults.toggleShrub()
    }

    @Then("^I see one test result with one value$")
    fun thenISeeOneTestResultWithOneValue() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with one value and a range$")
    fun thenISeeOneTestResultWithOneValueAndARange() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with multiple child values$")
    fun thenISeeOneTestResultWithMultipleChildValues() {
        assertTrue("Expected test result equal to or less than 1, but was" +
                "${myRecordInfoPage.testResults.allRecordItems().size}",
                myRecordInfoPage.testResults.allRecordItems().size >= 1)
        assertTrue("Expected child test result equal to or less than 1, but was " +
                "${myRecordInfoPage.getTestResultChildCount()}",
                myRecordInfoPage.getTestResultChildCount() > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRanges() {
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
    fun thenISeeTheTestResultHeading(getService: String) {
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
    fun thenISeeTheTestResultSectionCollapsed() {
        assertFalse(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see test result information$")
    fun thenISeeTestResultInformation() {
        assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see current repeat medication information$")
    fun thenISeeCurrentRepeatMedicationInformation() {
        assertTrue(myRecordInfoPage.repeatMedications.firstElement.element.isVisible)
    }

    @Then("^I see discontinued repeat medication information$")
    fun thenISeeDiscontinuedRepeatMedicationInformation() {
        assertTrue(myRecordInfoPage.discontinuedRepeatMedications.firstElement.element.isVisible)
    }

    @Then("^I see a message indicating that I have no access to view my summary care record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewMyRecord() {
        assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see immunisation records displayed$")
    fun thenISeeImmunisationRecordsDisplayed() {
        assertEquals(2, myRecordInfoPage.immunisations.allRecordItems().count())
    }

    @Then("^I see Problems records displayed$")
    fun thenISeeProblemsRecordsDisplayed() {
        assertEquals(NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED, myRecordInfoPage.problems.allRecordItems().count())
    }

    @Then("^I see Consultations records displayed$")
    fun thenISeeConsultationsRecordsDisplayed() {
        assertEquals(2, myRecordInfoPage.consultations.allRecordItems().count())
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        assertEquals("Expected test results", count, myRecordInfoPage.testResults.allRecordItems().count())
    }

    @Then("^I see a message indicating that I have no access to view (.*) on My Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSection(heading: String) {
        assertTextInSection(heading, "You do not currently have access to this section")
    }

    @Then("^I see an error occurred message with (.*) on My Record$")
    fun thenISeeAnErrorOccuredMessageForProblems(heading: String) {
        assertTextInSection(heading, "An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message indicating that I have no information recorded for (.*) on My Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecorded(heading: String) {
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
    fun thenISeeADrugAndNonDrugAllergyRecordFromVision() {
        val allergyMessages = myRecordInfoPage.allergies.allRecordItemBodies()
        val expectedMessages = listOf(
                "H/O: drug allergy",
                "Paracetamol 500mg capsules",
                "Leg swelling",
                "Pollen"
        )
        assertTrue("Expected records", allergyMessages.size == expectedMessages.size)
        allergyMessages.forEachIndexed { i, message -> assertTrue(message == expectedMessages[i]) }
    }

    @Then("I see the my record page scrolled to the test result section")
    fun thenISeeMyRecordPageScrolledToTestResultSection() {
        assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }
}
