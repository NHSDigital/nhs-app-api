package pages.appointments

import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/confirmation")
open class AppointmentsConfirmationPage : AppointmentSharedElementsPage() {

    private val inputXpathPrefix = "//input"
    private val typeRadioXpathPart = "@type='radio'"
    private val inputTypeRadioXpathPrefix = "$inputXpathPrefix[$typeRadioXpathPart"
    private val telephoneNumberRadioButtonByValueXpath = "$inputTypeRadioXpathPrefix and @value='%s']"
    private val alternateNumberRadioButtonXpath = "$inputTypeRadioXpathPrefix and @id='otherPhoneNumberRadioInput']"

    val telephoneNumberDiv = HybridPageElement(
            browserLocator = "//input[@id='telephoneNumberText']",
            androidLocator = null,
            page = this
    )

    val reasonFormField = HybridPageElement(
            browserLocator = "//textarea[@id='reasonText']",
            androidLocator = null,
            page = this
    )

    val radioButtons = HybridPageElement(
            browserLocator = "$inputTypeRadioXpathPrefix]",
            androidLocator = null,
            page = this
    )

    private fun telephoneNumberRadioButton(telephoneNumber: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = String.format(telephoneNumberRadioButtonByValueXpath, telephoneNumber),
                androidLocator = null,
                page = this
        )
    }

    private fun telephoneNumberRadioButtonSection(telephoneNumber: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = String.format("$telephoneNumberRadioButtonByValueXpath/..", telephoneNumber),
                androidLocator = null,
                page = this
        )
    }

    private val alternateTelephoneNumberRadioButton = HybridPageElement(
            browserLocator = alternateNumberRadioButtonXpath,
            androidLocator = null,
            page = this
    )

    private val alternateTelephoneNumberRadioButtonSection = HybridPageElement(
            browserLocator = "$alternateNumberRadioButtonXpath/..",
            androidLocator = null,
            page = this
    )

    private val selectedAppointmentParentXpath = "//div[@aria-label='selected appointment']"

    val telephoneError = HybridPageElement(
            browserLocator = "//*[@data-purpose='telephone-error']",
            androidLocator = null,
            page = this,
            helpfulName = "Telephone Error"
    )

    private fun getTelephoneNumberRadioButtonText(telephoneNumber: String): String {
        return telephoneNumberRadioButtonSection(telephoneNumber).element.text.trim()
    }

    fun describeSymptoms(symptoms: String) {
        reasonFormField.element.type<WebElementFacade>(symptoms)
        hideKeyboardIfOnMobile()
    }

    fun pasteSymptoms(symptoms: String) {
        reasonFormField.element.sendKeys(symptoms)
        hideKeyboardIfOnMobile()
    }

    fun getSymptoms(): String {
        return reasonFormField.element.value
    }

    fun describeTelephoneNumber(telephoneNumber: String) {
        telephoneNumberDiv.element.type<WebElementFacade>(telephoneNumber)
        hideKeyboardIfOnMobile()
    }

    fun getAppointmentSlot(areCliniciansExpected: Boolean = false): Slot {
        val slotsArray = getAllSlots(selectedAppointmentParentXpath, areCliniciansExpected)
        return slotsArray[0]
    }

    fun assertRadioButtonDisplayedForPhoneNumber(phoneNumber: String) {
        telephoneNumberRadioButtonSection(phoneNumber).assertIsVisible()
        Assert.assertEquals(
                "Phone number not displayed against the radio button. ",
                phoneNumber,
                getTelephoneNumberRadioButtonText(phoneNumber)
        )
    }

    fun assertRadioButtonDisplayedForAlternateNumber() {
        alternateTelephoneNumberRadioButtonSection.assertIsVisible()
        val expectedText = "Use other phone number"
        Assert.assertEquals(
                "Expected text for the radio button for entering an alternate number is incorrect. ",
                expectedText,
                alternateTelephoneNumberRadioButtonSection.element.text
        )
    }

    fun getNumberOfSelectedPhoneNumberRadioButtons(): Int {
        return radioButtons.elements.count { radioButton ->
            radioButton.isSelected
        }
    }

    fun selectPhoneNumberRadioButtonByText(telephoneNumber: String) {
        telephoneNumberRadioButton(telephoneNumber).element.click()
    }

    fun selectRadioButtonForAlternativePhoneNumber() {
        alternateTelephoneNumberRadioButton.element.click()
    }

    fun assertRadioButtonForAlternativePhoneNumberIsSelected() {
        Assert.assertTrue("Radio button for alternate telephone number is not selected. " +
                "${getNumberOfSelectedPhoneNumberRadioButtons()} " +
                "radio buttons are selected. ",
                alternateTelephoneNumberRadioButton.element.isSelected
        )
    }
}
