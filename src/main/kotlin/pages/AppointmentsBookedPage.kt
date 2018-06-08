package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By

@DefaultUrl("http://localhost:3000/appointments/booked")
open class AppointmentsBookedPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun getSuccessMessage(): String
    {
        return find<WebElementFacade>(By.id("success-dialog")).text
    }
}