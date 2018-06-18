package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.JavascriptExecutor

@DefaultUrl("http://localhost:3000/appointments")
open class MyAppointmentsPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun getSuccessMessage(): String
    {
        return find<WebElementFacade>(By.id("success-dialog")).text
    }

    fun clickOnButton(button: String)
    {
        val element = find<WebElementFacade>(By.ByXPath("//button[contains(text(),'$button')]"))
        element.waitUntilVisible<WebElementFacade>()

        val jsExecutor = driver as JavascriptExecutor
        jsExecutor.executeScript("arguments[0].scrollIntoView(true);", element)

        element.click()
    }

    fun getHeader(): String{
        return find<WebElementFacade>(By.ByCssSelector("header")).find<WebElementFacade>(By.cssSelector("h1")).text
    }
}
