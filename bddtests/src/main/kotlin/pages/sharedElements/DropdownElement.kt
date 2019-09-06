package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.selectedVisibleTextValue
import pages.assertElementNotPresent
import pages.text

const val DROPDOWN_WAIT_TIME = 60

class DropdownElement(val label: String, val helpfulName: String, pageObject: HybridPageObject) {

    private val xpathFormat = "//label[contains(text(),'$label')]/following-sibling::span/select"
    private val xpathFormatForInlineError =
            "//label[contains(text(),'$label')]/following-sibling::p/span[@data-purpose='error']"

    private val dropDown = HybridPageElement(
            webDesktopLocator = xpathFormat,
            page = pageObject,
            helpfulName = helpfulName,
            timeToWaitForElement = DROPDOWN_WAIT_TIME
    )

    private val inlineError by lazy {
        HybridPageElement(
                webDesktopLocator = xpathFormatForInlineError,
                page = pageObject,
                helpfulName = helpfulName
        )
    }

    fun assertIsVisible() {
        dropDown.assertIsVisible()
    }

    fun selectByText(text: String) {
        assertIsVisible()
        dropDown.actOnTheElement {
            it.selectByVisibleText<WebElementFacade>(text)
            assertSelected(text)
        }
    }

    fun assertContents(expectedContents: List<String>) {
        dropDown.actOnTheElement {
            val actualContents = it.selectOptions.map { value -> value.trim() }
            val message = "Expected list of $helpfulName. " +
                    "Expected: ${expectedContents.joinToString()}. " +
                    "Actual: ${actualContents.joinToString()}."

            Assert.assertEquals(message, expectedContents.distinct().count(), actualContents.count())
            Assert.assertTrue(message, actualContents.containsAll(expectedContents))
        }
    }

    fun getSelectedValue(): String {
        return dropDown.selectedVisibleTextValue.trim()
    }

    fun getContents(): ArrayList<String> {
        val optionsAsStrings = arrayListOf<String>()

        dropDown.actOnTheElement {
            val optionElements = it.findElements<WebElement>(By.xpath("./*"))

            for (option in optionElements) {
                optionsAsStrings.add(option.text.trim())
            }
        }
        return optionsAsStrings
    }

    fun assertSelected(expected: String) {
        Assert.assertEquals("Expected Value for $label", expected, getSelectedValue())
    }

    fun assertNotPresent() {
        dropDown.assertElementNotPresent()
    }

    fun assertSortedContent(defaultDropdownValue: String, expectedDropDownContent: ArrayList<String>) {
        val retrievedDropDownContent = getContents()
        expectedDropDownContent.sorted()
        expectedDropDownContent.add(0, defaultDropdownValue)
        val message = "Expected list of $helpfulName. " +
                "Expected: ${expectedDropDownContent.joinToString()}. " +
                "Actual: ${retrievedDropDownContent.joinToString()}."
        Assert.assertEquals(message, expectedDropDownContent, retrievedDropDownContent)
    }

    fun assertInlineErrorContent(expectedInlineErrorText: String){
        val retrievedInlineText = inlineError.text
        val message = "Expected Inline error text $helpfulName."
        Assert.assertEquals(message, expectedInlineErrorText, retrievedInlineText)
    }
}
