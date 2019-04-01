package pages

import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement

const val WAIT_FOR_NON_STALE_ELEMENT = 500L

fun HybridPageElement.waitForNonStaleElementToBecomeVisible() : WebElement{
    var staleElement = true
    var wrappedElement: WebElement? = null
    while(staleElement) {
        try {
            wrappedElement = this.element.wrappedElement
            if(wrappedElement.size!=null || this.element.isVisible)
                staleElement = false
        }
        catch(e: StaleElementReferenceException) {
            Thread.sleep(WAIT_FOR_NON_STALE_ELEMENT)
            staleElement = true
        }
    }
    return wrappedElement!!
}

open class LocatorMethods(var page:HybridPageObject) {

    fun waitForNativeStepToComplete(milliseconds: Long = DEFAULT_MOBILE_WAIT) {
        if (page.onMobile()) {
            Thread.sleep(milliseconds)
        }
    }

    fun assertNativeElementsLoaded(elementToCheck:HybridPageElement){
        if(page.onMobile()) {
            elementToCheck.assertIsVisible()
        }
    }
}