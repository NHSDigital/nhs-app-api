package pages.onlineConsultations

import mocking.onlineConsultations.constants.OnlineConsultationConstants
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.DropdownElement
import pages.text
import pages.typeTextIntoTextArea
import pages.value
import utils.SerenityHelpers

class OnlineConsultationsPage: HybridPageObject() {

    private val firstName: String = SerenityHelpers.getPatient().firstName

    private val expectedAdviceText = "Emergency advice:\nYou've chosen to end your consultation. Your practice hasn't" +
            " been notified and won't contact you about your request. You should still seek medical advice now."

    private val expectedCarePlanText = "Thank you $firstName. " +
            "The answers to your consultation have been securely sent to your GPs at Integration Test Practice."

    fun endMyConsultationButtonClicked() {
        HybridPageElement(webDesktopLocator = "//button[contains(text(),'End my consultation')]",
                          page = this).click()
    }

    fun iSeeAdviceOnWhatToDoNext(){
        val adviceText = HybridPageElement(
                webDesktopLocator = "//span[@role='text']",
                page = this)
                .text
        Assert.assertEquals(expectedAdviceText, adviceText)
    }

    fun iSeeACarePlan() {
        val carePlanText = HybridPageElement(
                webDesktopLocator = "//div[@id='question']//h1",
                page = this)
                .text
        Assert.assertEquals(expectedCarePlanText, carePlanText)
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

    private fun dateOfBirthElement(id: String, expectedValue: String){
        val actualText = HybridPageElement(webDesktopLocator = "//input" +
                "[@id='${OnlineConsultationConstants.DATE_OF_BIRTH_INPUT}-$id']", page = this).value
        Assert.assertEquals(actualText, expectedValue)
    }
}