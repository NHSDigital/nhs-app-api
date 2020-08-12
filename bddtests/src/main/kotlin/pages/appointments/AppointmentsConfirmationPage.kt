package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.typeTextIntoTextArea

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/confirmation")
open class AppointmentsConfirmationPage : AppointmentSharedElementsPage() {
    val notAvailableTitle = "Sorry, the GP appointment is no longer available"
    val chooseDifferent = "Please choose a different appointment."
    val reachedLimitTitle = "Sorry, you have reached your GP appointment limit"
    val cannotBook = "You cannot book any more appointments right now."
    val contactGp = "Contact your GP surgery if you still need to book an appointment."
    val cancelNoLongerNeeded = "You can go back to see what you have already booked and cancel any appointments " +
            "that you may no longer need."
    val urgentAdvice = "For urgent medical advice, go to 111.nhs.uk or call 111."
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

    private val alternateTelephoneNumberRadioButton = HybridPageElement(
            webDesktopLocator = alternateNumberRadioButtonXpath,
            webMobileLocator = alternateNumberRadioButtonXpath,
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

    override val titleText: String = "Confirm your appointment"

    fun describeSymptoms(symptoms: String) {
        reasonFormField.typeTextIntoTextArea(symptoms)
        hideKeyboardIfOnMobile()
    }

    fun describeTelephoneNumber(telephoneNumber: String) {
        telephoneNumberDiv.actOnTheElement { it.type<WebElementFacade>(telephoneNumber) }
        hideKeyboardIfOnMobile()
    }

    fun getNumberOfSelectedPhoneNumberRadioButtons(): Int {
        return radioButtons.elements.count { radioButton ->
            radioButton.isSelected
        }
    }

    fun selectPhoneNumberRadioButtonByText(telephoneNumber: String) {
        telephoneNumberRadioButton(telephoneNumber).click()
    }

    fun selectRadioButtonForAlternativePhoneNumber() {
        alternateTelephoneNumberRadioButton.click()
    }
}
