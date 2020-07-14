package pages

import org.openqa.selenium.WebDriverException

open class CheckMySymptomsPage : HybridPageObject() {

    private val conditionsHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Search conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    private val nhs111Header = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Use NHS 111 online')]",
            androidLocator = null,
            page = this
    )

    private val coronaVirusHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Get advice about coronavirus')]",
            androidLocator = null,
            page = this
    )

    private val gpAdviceHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Ask your GP for advice')]",
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

    fun isCoronaHeaderVisible(): Boolean {
        return coronaVirusHeader.isDisplayed
    }

    fun clickNHS111Header(){
        nhs111Header.click()
    }

    fun clickConditionsHeader() {
        conditionsHeader.click()
    }

    fun clickCoronaVirusHeader() {
        coronaVirusHeader.click()
    }

    fun clickGPAdviceHeader(){
        gpAdviceHeader.click()
    }
}
