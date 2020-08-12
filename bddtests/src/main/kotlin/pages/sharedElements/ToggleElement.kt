package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.MILLISECONDS_IN_A_SECOND
import pages.waitUntilPresent

const val NUMBER_OF_RETRIES = 10
class ToggleElement(val page : HybridPageObject, text:String, id:String) {

    private val xPathFromContainerToToggleInput = "div/input[@type='checkbox']"

    private val toggleElement = HybridPageElement(
            webDesktopLocator = "//div[div[input[@type='checkbox']]][label[contains(normalize-space(),'$text')]]",
            page = page)

    private val toggleSpinner = HybridPageElement(
            webDesktopLocator = "//*[@id='${id}_spinner']",
            page = page)

    fun assertIsVisible() {
        toggleElement.waitUntilPresent()
    }

    fun click() {
        toggleElement.actOnTheElement { it.findElement<WebElement>(By.xpath("./label")).click() }
    }

    fun assertOn() {
        assertState(true)
    }

    fun assertOff() {
        assertState(false)
    }

    private fun isLoading(): Boolean {
        toggleSpinner.waitUntilPresent()
        var isVisible = false
        toggleSpinner.actOnTheElement { isVisible = it.isCurrentlyVisible }
        return isVisible
    }

    private fun assertState(expectedChecked: Boolean) {
        waitForLoadingToComplete()
        var actualState :Boolean? = null
        var retryCounter = NUMBER_OF_RETRIES
        while (retryCounter >= 0) {
            try {
                if (retryCounter == 0) {
                    Assert.fail("Expected toggle state: $expectedChecked. Actual $actualState")
                }
                if (expectedChecked!=actualState) {
                    retryCounter--
                    toggleElement.actOnTheElement {
                        val input = it.findElement<WebElement>(By.xpath("./$xPathFromContainerToToggleInput"))
                        actualState = input.isSelected
                    }
                    Thread.sleep(MILLISECONDS_IN_A_SECOND)
                } else {
                    break
                }
            } catch (e: StaleElementReferenceException) {
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
        }
    }

    private fun waitForLoadingToComplete() {
        var retryCounter = NUMBER_OF_RETRIES
        while (retryCounter >= 0) {
            try {
                if (retryCounter == 0) {
                    Assert.fail("Toggle loading failed to complete in given time.")
                }
                if (isLoading()) {
                    retryCounter--
                    Thread.sleep(MILLISECONDS_IN_A_SECOND)
                } else {
                    break
                }
            } catch (e: StaleElementReferenceException) {
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
        }
    }
}
