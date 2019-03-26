package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
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
        checkBoxElement.element.findElement(By.xpath("./label")).click()
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
        Assert.assertEquals("Expected error message", errorMessage, inlineErrors.single().text)
    }

    fun assertNoInlineError() {
        Assert.assertEquals("Expected no inline error", 0, getInlineError().count())
    }

    private fun getInlineError(): List<WebElement> {
        return checkBoxElement.element
                .findElements(By.xpath("./preceding-sibling::p[contains(@class, error-message)]/span"))
    }

    private fun getCheckedElementsCount(): Int {
        val elements = checkBoxElement.element.findElements(By.xpath("./div//*[local-name() = 'svg']"))
        return elements.count()
    }
}