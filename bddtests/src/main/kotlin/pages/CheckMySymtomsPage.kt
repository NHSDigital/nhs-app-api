package pages

import com.gargoylesoftware.htmlunit.ElementNotFoundException
import io.appium.java_client.ios.IOSDriver
import net.thucydides.core.webdriver.WebDriverFacade

open class CheckMySymtomsPage : HybridPageObject() {

    val conditionsHeader = HybridPageElement(
            browserLocator = "//h2[contains(text(),'A-Z of conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    val nhs111Header = HybridPageElement(
            browserLocator = "//h2[contains(text(),'Check if I need urgent help')]",
            androidLocator = null,
            page = this
    )

    fun isConditionsHeaderVisible(): Boolean {
        switchWebview()
        for(window in driver.windowHandles.toList()) {
            driver.switchTo().window(window)
            try {
                if (conditionsHeader.element.isDisplayed)
                    return true
            }
            catch(e: ElementNotFoundException){
                println("Did not find element; switching window")
            }
        }
        return false
    }

    fun isNhs111HeaderVisible(): Boolean {
        return nhs111Header.element.isDisplayed
    }
}
