package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent

class CheckBoxElement(val page : HybridPageObject, text:String) {

    private val checkBoxElement = HybridPageElement(
            webDesktopLocator = "//div[input[@type='checkbox']][label[contains(normalize-space(),'$text')]]",
            page = page)

    fun assertIsVisible() {
        checkBoxElement.assertSingleElementPresent().assertIsVisible()
    }

    fun click() {
        checkBoxElement.actOnTheElement { it.findElement(By.xpath("./label")).click() }
    }

    fun assertChecked() {
        assertChecked(true)
    }

    fun assertUnchecked() {
        assertChecked(false)
    }

    fun assertInlineError(errorMessage: String) {
        val inlineErrors = getInlineError()
        Assert.assertEquals("Expected inline error", 1, inlineErrors.count())
        Assert.assertEquals("Expected error message", errorMessage, inlineErrors.single())
    }

    fun assertNoInlineError() {
        Assert.assertEquals("Expected no inline error", 0, getInlineError().count())
    }

    private fun getInlineError(): List<String> {
        val inlineErrors = mutableListOf<String>()

        checkBoxElement.actOnTheElement {
            inlineErrors.addAll(
                    it.findElements(By.xpath("./preceding-sibling::p[contains(@class, error-message)]/span"))
                            .map { webElement -> webElement.text }
            )
        }

        return inlineErrors
    }

    private fun assertChecked(expectedChecked: Boolean) {
        val errorMessage = if (expectedChecked)
            "Expected checkbox to be checked"
        else "Expected checkbox to be unchecked"
        checkBoxElement.actOnTheElement {
            val input = it.findElement(By.xpath("./input"))
            val isSelected = input.isSelected
            Assert.assertEquals(errorMessage, expectedChecked, isSelected)
        }
    }
}