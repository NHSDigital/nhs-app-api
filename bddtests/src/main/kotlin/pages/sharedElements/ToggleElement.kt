package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.MILLISECONDS_IN_A_SECOND
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.isCurrentlyVisible

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
        toggleElement.assertSingleElementPresent().assertIsVisible()
    }

    fun click() {
        toggleElement.actOnTheElement { it.findElement<WebElement>(By.xpath("./label")).click() }
    }

    fun assertEnabled() {
        assertEnabled(true)
    }

    fun assertDisabled() {
        assertEnabled(false)
    }

    private fun isLoading(): Boolean {
        return toggleSpinner.isCurrentlyVisible
    }

    private fun assertEnabled(expectedChecked: Boolean) {
        waitForLoadingToComplete()
        val errorMessage = "Expected toggle 'checked':"
        toggleElement.actOnTheElement {
            val input = it.findElement<WebElement>(By.xpath("./$xPathFromContainerToToggleInput"))
            val isSelected = input.isSelected
            Assert.assertEquals(errorMessage, expectedChecked, isSelected)
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
