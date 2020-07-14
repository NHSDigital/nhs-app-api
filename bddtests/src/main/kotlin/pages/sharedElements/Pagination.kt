package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertSingleElementPresent

class Pagination(private val direction: String,
                 page: HybridPageObject) {

    val container = HybridPageElement(
            "//nav[@class='nhsuk-pagination']//li/a[span[normalize-space(text())='$direction']]",
            page = page,
            helpfulName = "Pagination")

    fun assertSingleElementPresent(pageTitle: String) {
        container.assertSingleElementPresent()
        container.actOnTheElement {
            Assert.assertEquals("Pagination '$direction' page",
                    pageTitle,
                    it.findElement<WebElement>(By.xpath(
                            "span[@class='nhsuk-pagination__page']")).text)
        }
    }

    fun click() {
        container.click()
    }

    fun assertElementNotPresent() {
        container.assertElementNotPresent()
    }
}
