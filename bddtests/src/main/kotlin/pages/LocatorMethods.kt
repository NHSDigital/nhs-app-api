package pages

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.support.ui.FluentWait
import java.time.Duration

fun HybridPageElement.waitForNonStaleElementToBecomeVisible() : HybridPageElement{
    var staleElement = true
    while(staleElement) {
        try {
            this.assertIsVisible()
            staleElement = false
        }
        catch(e: StaleElementReferenceException) {
            staleElement = true
        }
    }
    return this
}

open class LocatorMethods(var page:HybridPageObject) {

    fun waitForNativeStepToComplete(milliseconds: Long = DEFAULT_MOBILE_WAIT) {
        if (page.onMobile()) {
            Thread.sleep(milliseconds)
        }
    }

    fun assertNativeElementsLoaded(elementToCheck:HybridPageElement){
        if(page.onMobile()) {
            elementToCheck.shouldBeVisibleOnNative()
        }
    }

    private fun HybridPageElement.shouldBeVisibleOnNative(seconds: Long = DEFAULT_VISIBILITY_WAIT){
        try {
            page.waitForSpinnerToDisappear()
            val currentElement = this.element
            FluentWait<WebElementFacade>(currentElement)
                    .withTimeout(Duration.ofSeconds(seconds))
                    .pollingEvery(Duration.ofMillis(POOLING_FREQUENCY))
                    .until {
                        currentElement.isPresent
                    }
            if (!currentElement.isVisible) {
                Thread.sleep(DEFAULT_MOBILE_WAIT)
            }
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("Element $this does not exist on the page.  " +
                    "Page source:\n${page.driver.pageSource}\n")
        }
    }
}