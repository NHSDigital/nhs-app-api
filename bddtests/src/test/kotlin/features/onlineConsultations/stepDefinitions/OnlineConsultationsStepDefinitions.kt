package features.onlineConsultations.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.onlineConsultations.factories.OnlineConsultationsFactory
import mocking.onlineConsultations.constants.OnlineConsultationConstants
import pages.MILLISECONDS_IN_A_SECOND
import pages.PageLeavingWarning
import pages.onlineConsultations.OnlineConsultationsPage
import pages.onlineConsultations.OnlineConsultationsUnavailablePage
import pages.ServerError
import utils.SerenityHelpers

open class OnlineConsultationsStepDefinitions {

    private lateinit var onlineConsultationsPage: OnlineConsultationsPage
    private lateinit var onlineConsultationsUnavailablePage: OnlineConsultationsUnavailablePage
    private lateinit var pageLeaveWarning: PageLeavingWarning
    private lateinit var serverError: ServerError

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

    @Given("^I have access to online consultations gp advice journey for my child$")
    fun iHaveAccessToOnlineConsultationsForMyChild() {
        onlineConsultationsFactory.setupOnlineConsultationsDataChild()
    }

    @Given("I have access to the child journey when I am under 18$")
    fun iHaveAccessToChildJourneyWhenUnder18() {
        onlineConsultationsFactory.setUpOnlineConsultationsUnder18Message()
    }

    @Given("^I have access to online consultations gp advice journey and the response is delayed by (.*) seconds$")
    fun iHaveAccessToOnlineConsultationsNonEmergencyDelayedResponse(seconds: Int) {
        val timeOut = seconds * MILLISECONDS_IN_A_SECOND
        onlineConsultationsFactory.setupOnlineConsultationsDataNonEmergency(timeOut = timeOut.toInt())
    }

    @Given("^I have access to online consultations gp advice journey and it is not an emergency with no GP session$")
    fun iHaveAccessToOnlineConsultationsNonEmergencyWithNoGpSession() {
        onlineConsultationsFactory.setupOnlineConsultationsDataNonEmergency(false)
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

    @When("^I select my childs gender and click continue$")
    fun iSelectMyChildsGender(){
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

    @When("^I am submitting the questionnaire for my child$")
    fun iAmSubmittingTheQuestionnaireForChild() {
        onlineConsultationsPage.clickFormElement(id = OnlineConsultationConstants.CHILD_CHOICE)
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

    @When("^I insert my childs symptoms and click continue$")
    fun iInsertMyChildsSymptoms() {
        onlineConsultationsPage.insertText("textarea",
                OnlineConsultationConstants.HOW_CAN_WE_HELP_TEXTFIELD,
                "My child has a bad cough and a sore throat")
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

    @When("^I insert my childs date of birth$")
    fun iInsertMyChildsDateOfBirth(){
        onlineConsultationsPage.checkDateOfBirthPopulated("2004-02-01")
        continueClicked()
    }

    @When("^I select how much alcohol I drink weekly$")
    fun iSelectAlcoholAmount(){
        answerQuestionAndContinue(elementId=OnlineConsultationConstants.ALCOHOL_CHOICE)
    }

    @When("^I insert how long I have felt the pain$")
    fun iInsertHowLongIHaveFeltThePain() {
        onlineConsultationsPage.insertText(id=OnlineConsultationConstants.QUANTITY_NUMBER_INPUT, text="5")
        onlineConsultationsPage.selectUnit("mins")
        continueClicked()
    }

    @When("^I insert how long my child has had symptoms$")
    fun iInsertHowLongMyChildHasHadSymptoms() {
        onlineConsultationsPage.insertText(id=OnlineConsultationConstants.QUANTITY_NUMBER_INPUT, text="5")
        onlineConsultationsPage.selectUnit("days")
        continueClicked()
    }

    @Then("^I see advice on what to do next$")
    fun iSeeAdviceOnWhatToDoNext(){
        onlineConsultationsPage.iSeeAdviceOnWhatToDoNext()
    }

    @Then("^I see a message informing me I am too young$")
    fun iSeeAMessageThatIAmTooYoung() {
        onlineConsultationsPage.iSeeAMessageForTooYoung()
    }

    @Then("^I see a care plan for (myself|my child)$")
    fun iSeeACarePlanForMyChild(type: String) {
        onlineConsultationsPage.iSeeACarePlan(type)
    }

    @Then("^I see a condition list for (myself|my child)$")
    fun iSeeAConditionList(type: String) {
        onlineConsultationsPage.iSeeTheConditionListTitle(type)
        onlineConsultationsPage.iSeeTheCannotFindConditionLink(type)
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

    @Then("^I see the appropriate error message for an online consultation timeout$")
    fun iSeeTheAppropriateErrorMessageForAnOnlineConsultationsTimeout() {
        serverError.assert()
    }

    private fun answerQuestionAndContinue(elementType: String = "input", elementId: String){
        onlineConsultationsPage.clickFormElement(elementType, elementId)
        continueClicked()
    }

    private fun continueClicked() {
        onlineConsultationsPage.clickFormElement("button", OnlineConsultationConstants.CONTINUE_BUTTON)
    }
}
