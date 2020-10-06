package features.onlineConsultations.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.onlineConsultations.factories.OnlineConsultationsFactory
import mocking.onlineConsultations.constants.OnlineConsultationConstants
import pages.PageLeavingWarning
import pages.navigation.WebHeader
import pages.onlineConsultations.OnlineConsultationsPage
import pages.onlineConsultations.OnlineConsultationsUnavailablePage
import utils.SerenityHelpers

open class OnlineConsultationsStepDefinitions {

    private lateinit var onlineConsultationsPage: OnlineConsultationsPage
    private lateinit var onlineConsultationsUnavailablePage: OnlineConsultationsUnavailablePage
    private lateinit var pageLeaveWarning: PageLeavingWarning
    private lateinit var webHeader: WebHeader

    private val onlineConsultationsFactory = OnlineConsultationsFactory()

    @Given("^I have access to online consultations but they are switched off by the practice$")
    fun iHaveAccessToOnlineConsultationsButItIsSwitchedOffByThePractice() {
        onlineConsultationsFactory.setupOnlineConsultationsDataIsNotValid()
    }

    @Given("^I have access to online consultations gp advice journey and it is an emergency$")
    fun iHaveAccessToOnlineConsultationsAndItIsAnEmergency() {
        onlineConsultationsFactory.setupOnlineConsultationsData()
    }

    @Given("^I have access to online consultations gp advice journey and it is not an emergency$")
    fun iHaveAccessToOnlineConsultationsNonEmergency() {
        onlineConsultationsFactory.setupOnlineConsultationsDataNonEmergency()
    }

    @Given("^I have access to online consultations gp advice journey and it is not an emergency with no GP session$")
    fun iHaveAccessToOnlineConsultationsNonEmergencyWithNoGpSession() {
        onlineConsultationsFactory.setupOnlineConsultationsDataNonEmergency(false)
    }

    @Given("^I click on the More link on the header$")
    fun iClickOnTheMoreLinkOnTheHeader() {
        webHeader.clickMorePageLink()
    }

    @When("^I accept demographics and terms and conditions question$")
    fun iAcceptTheDemographics() {
        onlineConsultationsPage.clickFormElement(id=OnlineConsultationConstants.DEMOGRAPHICS_CHECKBOX)
        onlineConsultationsPage.clickFormElement("button", OnlineConsultationConstants.DEMOGRAPHICS_BUTTON)
        answerQuestionAndContinue(elementId = OnlineConsultationConstants.TERMS_AND_CONDITONS_CHECKBOX)
    }

    @When("^I select my gender and click continue$")
    fun iSelectMyGender(){
        answerQuestionAndContinue(elementId = OnlineConsultationConstants.MALE_GENDER_CHOICE)
    }

    @When("^I click the end my consultation button$")
    fun iClickTheEndMyConsultationButton(){
        onlineConsultationsPage.endMyConsultationButtonClicked()
    }

    @When("^I click on a condition$")
    fun iClickACondition() {
        answerQuestionAndContinue("button", OnlineConsultationConstants
                .BREATHING_PROBLEMS_CONDITION_ID)
    }

    @When("^I am submitting the questionnaire for myself$")
    fun iAmSubmittingTheQuestionnaireForMyself() {
        onlineConsultationsPage.clickFormElement(id = OnlineConsultationConstants.SELF_CHOICE)
        continueClicked()
    }

    @When("^I am not in an emergency$")
    fun iAmNotInAnEmergency(){
        answerQuestionAndContinue(elementId = OnlineConsultationConstants.NON_EMERGENCY_CHOICE)
    }

    @When("^I insert my symptoms and click continue$")
    fun iInsertMySymptoms() {
        onlineConsultationsPage.insertText("textarea",
                                           OnlineConsultationConstants.HOW_CAN_WE_HELP_TEXTFIELD,
                                           "I need medication for my sore throat")
        continueClicked()
    }

    @When("^I am in an emergency and I agree to end my consultation$")
    fun iAgreeToEndMyConsultation(){
        answerQuestionAndContinue(elementId = OnlineConsultationConstants.EMERGENCY_CHOICE)
        answerQuestionAndContinue(elementId=OnlineConsultationConstants.EMERGENCY_ACCEPTANCE_CHECKBOX)
    }

    @When("^I insert my date of birth$")
    fun iInsertMyDateOfBirth(){
        val patient = SerenityHelpers.getPatient()
        onlineConsultationsPage.checkDateOfBirthPopulated(patient.age.dateOfBirth)
        continueClicked()
    }

    @When("^I select how much alcohol I drink weekly$")
    fun iSelectAlcoholAmount(){
        answerQuestionAndContinue(elementId=OnlineConsultationConstants.ALCOHOL_CHOICE)
    }

    @When("^I click the origin of the pain on the image$")
    fun iClickAPointOnTheImage(){
        answerQuestionAndContinue("img", OnlineConsultationConstants.IMAGE)
    }

    @When("^I insert how long I have felt the pain$")
    fun iInsertHowLongIHaveFeltThePain() {
        onlineConsultationsPage.insertText(id=OnlineConsultationConstants.QUANTITY_NUMBER_INPUT, text="5")
        onlineConsultationsPage.selectUnit("mins")
        continueClicked()
    }

    @Then("^I see advice on what to do next$")
    fun iSeeAdviceOnWhatToDoNext(){
        onlineConsultationsPage.iSeeAdviceOnWhatToDoNext()
    }

    @Then("^I see a care plan$")
    fun iSeeACarePlan(){
        onlineConsultationsPage.iSeeACarePlan()
    }

    @Then("^I see the online consultations unavailable message for (gp advice|admin help)$")
    fun iSeeTheOnlineConsultationsSwitchedOffErrorMessages(journey: String) {
        onlineConsultationsUnavailablePage.assertIsVisible(journey == "gp advice")
    }

    @Then("^I see the page leave warning$")
    fun iSeeThePageLeaveWarning() {
        pageLeaveWarning.assertIsDisplayed()
    }

    @Then("^I click stay on page on the popup$")
    fun iClickStayOnPage() {
        pageLeaveWarning.clickStay()
    }

    @Then("^I click leave the page on the popup$")
    fun iClickLeavePage() {
        pageLeaveWarning.clickLeave()
    }

    private fun answerQuestionAndContinue(elementType: String = "input", elementId: String){
        onlineConsultationsPage.clickFormElement(elementType, elementId)
        continueClicked()
    }

    private fun continueClicked() {
        onlineConsultationsPage.clickFormElement("button", OnlineConsultationConstants.CONTINUE_BUTTON)
    }
}
