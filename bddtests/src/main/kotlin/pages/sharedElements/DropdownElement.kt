package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertIsVisible

class DropdownElement(val label: String, val helpfulName: String, pageObject: HybridPageObject) {

    private val byIdXpathFormat = "//label[contains(text(),'%s')]/following-sibling::span/select"

    private val dropDown = HybridPageElement(
            webDesktopLocator = String.format(byIdXpathFormat, label),
            page = pageObject,
            helpfulName = helpfulName
    )

    fun assertIsVisible() {
        dropDown.assertIsVisible()
    }

    fun selectByText(text: String) {
        assertIsVisible()
        dropDown.element.selectByVisibleText<WebElementFacade>(text)
        assertSelected(text)
    }

    fun assertContents(expectedContents: ArrayList<String>) {
        val actualContents = getContents()
        val message = "Expected list of $helpfulName. " +
                "Expected: ${expectedContents.joinToString()}. " +
                "Actual: ${actualContents.joinToString()}."

        Assert.assertEquals(message, expectedContents.distinct().count(), actualContents.count())
        Assert.assertTrue(message, actualContents.containsAll(expectedContents))
    }

    fun getSelectedValue(): String {
        return dropDown.element.selectedVisibleTextValue.trim()
    }

    fun getContents(): ArrayList<String> {
        val optionElements = dropDown.element.findElements(By.xpath("./option"))
        val optionsAsStrings = arrayListOf<String>()
        for (option in optionElements) {
            optionsAsStrings.add(option.text.trim())
        }
        return optionsAsStrings
    }

    fun assertSelected(expected: String) {
        Assert.assertEquals("Expected Value for $label", expected, getSelectedValue())
    }

    fun assertNotPresent() {
        dropDown.assertElementNotPresent()
    }
}