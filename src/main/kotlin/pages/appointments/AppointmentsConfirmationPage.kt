package pages.appointments

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.JavascriptExecutor
import pages.HybridPageObject

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

    fun clickOnButton(button: String)
    {
        val element = find<WebElementFacade>(By.ByXPath("//button[contains(text(),'$button')]"))
        element.waitUntilVisible<WebElementFacade>()

        val jsExecutor = driver as JavascriptExecutor
        jsExecutor.executeScript("arguments[0].scrollIntoView(true);", element)

        element.click()
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
        return findByXpath("//div[@id='mainDiv']/div[@class='content']").waitUntilVisible<WebElementFacade>()
    }

    fun isButtonVisible(button: String): Boolean
    {
        return findBy<WebElementFacade>("//button[contains(text(),'$button')]").waitUntilVisible<WebElementFacade>().isCurrentlyVisible
    }
}
