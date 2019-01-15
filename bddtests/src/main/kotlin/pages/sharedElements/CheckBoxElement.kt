package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class CheckBoxElement(page : HybridPageObject, text:String) {

    private val checkBoxElement = HybridPageElement(
            browserLocator = "//div[input[@type='checkbox']][label[contains(normalize-space(),'$text')]]",
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

    private fun getCheckedElementsCount(): Int {
        val elements = checkBoxElement.element.findElements(By.xpath("./div//*[local-name() = 'svg']"))
        return elements.count()
    }
}