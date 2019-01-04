package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/confirmation")
open class AppointmentsConfirmationPage : AppointmentSharedElementsPage() {
    private val confirmAndBookAppointmentButton = HybridPageElement(
            browserLocator = "//*[@id='btn_book_appointment']",
            androidLocator = null,
            page = this
    )

    private val reasonFormField = HybridPageElement(
            browserLocator = "//*[@id='reasonText']",
            androidLocator = null,
            page = this
    )

    val backToMyAppointmentsButton = HybridPageElement(
            browserLocator = "//button[contains(text(),'Back to my appointments')]",
            androidLocator = null,
            page = this
    )

    val symptomsFormDiv = HybridPageElement(
            browserLocator = "//*[@id='reasonText']",
            androidLocator = null,
            page = this
    )

    val telephoneNumberDiv = HybridPageElement(
            browserLocator = "//*[@id='telephoneNumberText']",
            androidLocator = null,
            page = this
    )

    fun clickOnConfirmAndBookAppointmentButton() {
        confirmAndBookAppointmentButton.click()
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

    fun isButtonVisible(button: String): Boolean {
        return findBy<WebElementFacade>(
                "//button[contains(text()," +
                        "'$button')]").waitUntilVisible<WebElementFacade>().isCurrentlyVisible
    }

    fun describeTelephoneNumber(telephoneNumber: String) {
        telephoneNumberDiv.element.type<WebElementFacade>(telephoneNumber)
        hideKeyboardIfOnMobile()
    }
}
