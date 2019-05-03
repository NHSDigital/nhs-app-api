package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent

class CheckBoxElement(page : HybridPageObject, text:String) {

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
       Assert.assertEquals("Expected checked",1, getCheckedElementsCount())
    }

    fun assertUnchecked() {
        Assert.assertEquals("Expected unchecked",0, getCheckedElementsCount())
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

    private fun getCheckedElementsCount(): Int {
        var elementCount = 0
        checkBoxElement.actOnTheElement {
            elementCount = it.findElements(By.xpath("./div//*[local-name() = 'svg']")).count()
        }
        return elementCount
    }
}