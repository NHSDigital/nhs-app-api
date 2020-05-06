package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertSingleElementPresent

class LinkElement(page: HybridPageObject,
                  content: ILinksContent,
                  linkTitle: String,
                  private val description: String?) {


    private val linkPath = content.specificLinkXPath(linkTitle)
    private val link = HybridPageElement(
            webDesktopLocator = linkPath,
            page = page,
            helpfulName = "$linkTitle Link")

    fun assertSingleElementPresent() {
        link.assertSingleElementPresent()
        if (description != null) {
            link.actOnTheElement {
                val actualDescription = it.findElement<WebElement>(By.xpath("./p"))
                Assert.assertEquals("Link description", description, actualDescription.text)
            }
        }
        return
    }

    fun click() {
        link.click()
    }

    fun assertElementNotPresent(){
        link.assertElementNotPresent()
    }
}