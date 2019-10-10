package pages.sharedElements

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.text

class ExpandElement(pageObject: HybridPageObject){

    private val parentXpath = "//*[@data-purpose='info-msg']"
    private val guidanceIconXpathFormat = "$parentXpath//*[@data-purpose='icon']${pageObject
            .containsTextXpathSubstring}"

    private val expand = HybridPageElement(
            webDesktopLocator = String.format(guidanceIconXpathFormat, "+"),
            androidLocator = null,
            page = pageObject,
            helpfulName = "Expand icon"
    )

    private val collapse = HybridPageElement(
            // Note that the character is a true minus sign and not a hyphen
            webDesktopLocator = String.format(guidanceIconXpathFormat, "−"),
            androidLocator = null,
            page = pageObject,
            helpfulName = "Collapse icon"
    )

    private val label = HybridPageElement(
            webDesktopLocator = "$parentXpath//p",
            androidLocator = null,
            page = pageObject,
            helpfulName = "Section label. "
    )

    private val content = HybridPageElement(
            webDesktopLocator = "$parentXpath//*[@data-purpose='info-content']",
            androidLocator = null,
            page = pageObject,
            helpfulName = "Content. "
    )

    fun collapse(){
        collapse.click()
    }

    fun expand(){
        expand.click()
    }

    fun assertLabel(expectedLabel : String) {
        Assert.assertEquals("Label of expand section",
                expectedLabel,
                label.text.trim())
    }

    fun assertContent(expectedContent : String) {
        Assert.assertEquals("Content of expand section",
                expectedContent,
                content.text.trim())
    }

    fun assertCollapsed() {
        content.assertElementNotPresent()
    }

    fun assertNotPresent(){
        content.assertElementNotPresent()
        label.assertElementNotPresent()
    }
}
