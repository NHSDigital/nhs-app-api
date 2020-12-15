package pages.onlineConsultations

import mocking.onlineConsultations.constants.OnlineConsultationConstants
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.DropdownElement
import pages.typeTextIntoTextArea
import pages.value
import pages.withNormalisedText

class OnlineConsultationsPage: HybridPageObject() {

    private val expectedAdviceText =
            "You've chosen to end your consultation. " +
            "Your practice hasn't been notified and won't contact you about your request. " +
            "You should still seek medical advice now."

    private val childTitle = "To ensure we ask you relevant questions, choose your child's condition."
    private val childLink = "I cannot find my child's condition"

    private val adultTitle = "To ensure we ask you relevant questions, choose your condition."
    private val adultLink = "I cannot find my condition"

    private val expectedTooYoungMessage = "You must be between 18 and 125 years old in order to complete this request."

    private val expectedAdultCarePlanText = "If your condition gets worse while you're waiting for a " +
            "response, call the practice on 01234567890 as soon as possible. If the practice is closed, call NHS " +
            "111. For immediate, life-threatening emergencies, call 999."

    private val expectedChildCarePlanText = "If your child's condition gets worse while you're waiting for a " +
            "response, call the practice on 01234567890 as soon as possible. If the practice is closed, call NHS " +
            "111. For immediate, life-threatening emergencies, call 999."

    fun endMyConsultationButtonClicked() {
        HybridPageElement(webDesktopLocator = "//button[contains(text(),'End my consultation')]",
                          page = this).click()
    }

    fun iSeeAdviceOnWhatToDoNext(){
        assertVisualElement("span", text = expectedAdviceText)
    }

    fun iSeeACarePlan(type: String) {
        val carePlanText = if(type == "my child") expectedChildCarePlanText else expectedAdultCarePlanText
        assertVisualElement("div", "alert", carePlanText)
    }

    fun iSeeTheCannotFindConditionLink(type: String) {
        val linkText = if(type == "my child") childLink else adultLink
        assertVisualElement("a", "cannotFindConditionLink", linkText)
    }

    fun iSeeTheConditionListTitle(type: String) {
        val titleText = if(type == "my child") childTitle else adultTitle
        assertVisualElement("p", "conditionListTitle", titleText)
    }

    fun iSeeAMessageForTooYoung() {
        assertVisualElement("div", "question", expectedTooYoungMessage)
    }

    fun clickFormElement(element: String = "input", id: String) {
        return HybridPageElement(
                webDesktopLocator = "//$element[@id='$id']",
                page = this).click()
    }

    fun insertText(element: String = "input", id: String, text: String) {
        HybridPageElement("//$element[@id='$id']", page = this)
                .typeTextIntoTextArea(text)
    }

    fun checkDateOfBirthPopulated(dateOfBirth: String){
        val dobSplit = dateOfBirth.split("-")
        dateOfBirthElement("day", dobSplit[2])
        dateOfBirthElement("month", dobSplit[1])
        dateOfBirthElement("year", dobSplit[0])
    }

    fun selectUnit(unit: String) {
        DropdownElement("Unit", "QuantityUnitDropdown", this)
                .selectByText(unit)
    }


    private fun assertVisualElement(element: String, id: String? = null, text: String) {
        val webDesktopLocator = if (id != null) "//$element[@id='$id']" else "//$element"
        HybridPageElement(
                webDesktopLocator,
                page = this)
                .withNormalisedText(text)
                .assertIsVisible()
    }

    private fun dateOfBirthElement(id: String, expectedValue: String){
        val actualText = HybridPageElement(webDesktopLocator = "//input" +
                "[@id='${OnlineConsultationConstants.DATE_OF_BIRTH_INPUT}-$id']", page = this).value
        Assert.assertEquals(actualText, expectedValue)
    }
}
