package pages

import org.openqa.selenium.WebDriverException

open class CheckMySymptomsPage : HybridPageObject() {

    private val conditionsHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'A to Z of conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    private val nhs111Header = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Check if you need urgent help')]",
            androidLocator = null,
            page = this
    )

    fun isConditionsHeaderVisible(): Boolean {
        switchWebview()
        for(window in driver.windowHandles.toList()) {
            locatorMethods.waitForNativeStepToComplete()
            driver.switchTo().window(window)
            try {
                if (conditionsHeader.isDisplayed)
                    return true
            }
            catch(e: WebDriverException){
                println("Did not find element; switching window")
            }
        }
        return false
    }

    fun isNhs111HeaderVisible(): Boolean {
        return nhs111Header.isDisplayed
    }

    fun clickNHS111Header(){
        nhs111Header.click()
    }

    fun clickConditionsHeader() {
        conditionsHeader.click()
    }
}
