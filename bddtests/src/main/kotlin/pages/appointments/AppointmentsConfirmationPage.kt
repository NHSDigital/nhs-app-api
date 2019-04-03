package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/confirmation")
open class AppointmentsConfirmationPage : AppointmentSharedElementsPage() {

    private val inputXpathPrefix = "//input"
    private val typeRadioXpathPart = "@type='radio'"
    private val inputTypeRadioXpathPrefix = "$inputXpathPrefix[$typeRadioXpathPart"
    private val telephoneNumberRadioButtonByValueXpath = "$inputTypeRadioXpathPrefix and @value='%s']"
    private val alternateNumberRadioButtonXpath = "$inputTypeRadioXpathPrefix and @id='otherPhoneNumberRadioInput']"

    val reasonFormField = HybridPageElement(
            webDesktopLocator = "//textarea[@id='reasonText']",
            webMobileLocator = "//*[@id='reasonText']",
            androidLocator = null,
            page = this
    )

    val radioButtons = HybridPageElement(
            webDesktopLocator = "$inputTypeRadioXpathPrefix]",
            webMobileLocator = "$inputTypeRadioXpathPrefix]",
            androidLocator = null,
            page = this
    )

    private fun telephoneNumberRadioButton(telephoneNumber: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = String.format(telephoneNumberRadioButtonByValueXpath, telephoneNumber),
                webMobileLocator = String.format(telephoneNumberRadioButtonByValueXpath, telephoneNumber),
                androidLocator = null,
                page = this
        )
    }

    private fun telephoneNumberRadioButtonSection(telephoneNumber: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = String.format("$telephoneNumberRadioButtonByValueXpath/..", telephoneNumber),
                webMobileLocator = String.format("$telephoneNumberRadioButtonByValueXpath/..", telephoneNumber),
                androidLocator = null,
                page = this
        )
    }

    private val alternateTelephoneNumberRadioButton = HybridPageElement(
            webDesktopLocator = alternateNumberRadioButtonXpath,
            webMobileLocator = alternateNumberRadioButtonXpath,
            androidLocator = null,
            page = this
    )

    private val alternateTelephoneNumberRadioButtonSection = HybridPageElement(
            webDesktopLocator = "$alternateNumberRadioButtonXpath/..",
            webMobileLocator = "$alternateNumberRadioButtonXpath/..",
            androidLocator = null,
            page = this
    )

    val telephoneNumberDiv = HybridPageElement(
            webDesktopLocator = "//*[@id='telephoneNumberText']",
            webMobileLocator = "//*[@id='telephoneNumberText']",
            androidLocator = null,
            page = this
    )

    val telephoneError = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='telephone-error']",
            webMobileLocator = "//*[@data-purpose='telephone-error']",
            androidLocator = null,
            page = this,
            helpfulName = "Telephone Error"
    )

    override val titleText: String = "Confirm appointment"

    private fun getTelephoneNumberRadioButtonText(telephoneNumber: String): String {
        return telephoneNumberRadioButtonSection(telephoneNumber).element.text.trim()
    }

    fun describeSymptoms(symptoms: String) {
        reasonFormField.typeTextIntoTextArea(symptoms)
        hideKeyboardIfOnMobile()
    }

    fun getSymptoms(): String {
        return reasonFormField.element.value
    }

    fun describeTelephoneNumber(telephoneNumber: String) {
        telephoneNumberDiv.element.type<WebElementFacade>(telephoneNumber)
        hideKeyboardIfOnMobile()
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
