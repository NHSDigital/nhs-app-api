package pages.navigation

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject

@DefaultUrl("http://localhost:3000/appointments")
open class MyAppointmentsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(xpath = "//button[contains(text(),'Book new appointment')]")
    lateinit var bookNewAppointmentButton: WebElementFacade

    fun clickOnBookNewAppointmentButton()
    {
        bookNewAppointmentButton.waitUntilClickable<WebElementFacade>().click()
    }
}
