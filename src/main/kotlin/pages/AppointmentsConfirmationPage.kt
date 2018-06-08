package pages

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.JavascriptExecutor

@DefaultUrl("http://localhost:3000/appointments/confirmation")
open class AppointmentsConfirmationPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(id = "btn_book_appointment")
    lateinit var confirmAndBookAppointmentButton: WebElementFacade

    @FindBy(id = "reasonText")
    lateinit var reasonFormField: WebElementFacade

    fun clickOnConfirmAndBookAppointmentButton()
    {
        confirmAndBookAppointmentButton.waitUntilVisible<WebElementFacade>()

        val jsExecutor = driver as JavascriptExecutor
        jsExecutor.executeScript("arguments[0].scrollIntoView(true);", confirmAndBookAppointmentButton)

        confirmAndBookAppointmentButton.click()
    }

    fun getValidationErrorMessage(): WebElementFacade
    {
        return findBy<WebElementFacade>("#errorLabel p")
    }

    fun describeSymptoms(symptoms: String) {
        reasonFormField.type<WebElementFacade>(symptoms)
    }

    fun pasteSymptoms(symptoms: String) {
        reasonFormField.sendKeys(symptoms)
    }

    fun getSymptoms(): String {
        return reasonFormField.value
    }

    fun getServerErrorElement(): WebElementFacade {
        return findBy<WebElementFacade>("#serverError").waitUntilVisible<WebElementFacade>()
    }
}
