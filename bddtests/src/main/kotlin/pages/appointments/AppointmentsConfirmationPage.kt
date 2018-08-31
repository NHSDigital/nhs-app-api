package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.support.ui.WebDriverWait
import pages.HybridPageElement
import java.time.Duration

@DefaultUrl("http://localhost:3000/appointments/confirmation")
open class AppointmentsConfirmationPage: AppointmentSharedElementsPage() {

    val confirmAndBookAppointmentButton = HybridPageElement(
        browserLocator = "//*[@id='btn_book_appointment']",
        androidLocator = null,
        page = this
    )

    val reasonFormField = HybridPageElement(
            browserLocator = "//*[@id='reasonText']",
            androidLocator = null,
            page = this
    )

    val backToMyAppointmentsButton = HybridPageElement(
            browserLocator = "//button[contains(text(),'Back to my appointments')]",
            androidLocator = null,
            page = this
    )

    fun clickOnConfirmAndBookAppointmentButton() {
        confirmAndBookAppointmentButton.element.click()
    }

    fun describeSymptoms(symptoms: String) {
        reasonFormField.element.type<WebElementFacade>(symptoms)
        if (onMobile()) {
            hideKeyboard()
        }
    }

    fun pasteSymptoms(symptoms: String) {
        reasonFormField.element.sendKeys(symptoms)
        if (onMobile()) {
            hideKeyboard()
        }
    }

    fun getSymptoms(): String {
        return reasonFormField.element.value
    }

    fun isButtonVisible(button: String): Boolean {
        return findBy<WebElementFacade>(
                "//button[contains(text()," +
                "'$button')]").waitUntilVisible<WebElementFacade>().isCurrentlyVisible
    }
}
