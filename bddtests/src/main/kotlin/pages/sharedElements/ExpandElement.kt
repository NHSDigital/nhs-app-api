package pages.sharedElements

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.text

class ExpandElement(pageObject: HybridPageObject) {

    private val parentXpath = "//details[contains(@class, 'nhsuk-details nhsuk-expander')]"

    private val summaryXpath = "$parentXpath/summary"

    private val expander = HybridPageElement(
            webDesktopLocator = parentXpath,
            androidLocator = null,
            page = pageObject,
            helpfulName = "Details Expand Element"
    )

    private val summary = HybridPageElement(
            webDesktopLocator = summaryXpath,
            androidLocator = null,
            page = pageObject,
            helpfulName = "Details Expand Summary"
    )

    private val content = HybridPageElement(
            webDesktopLocator = "$parentXpath/div",
            androidLocator = null,
            page = pageObject,
            helpfulName = "Details Expand Content"
    )

    fun collapse() {
        summary.click()
        assertCollapsed()
    }

    fun expand() {
        summary.click()
        assertExpanded()
    }

    fun assertLabel(expectedLabel: String) {
        Assert.assertEquals("Label of expand section",
                expectedLabel,
                summary.text.trim())
    }

    fun assertContent(expectedContent: String) {
        Assert.assertEquals("Content of expand section",
                expectedContent,
                content.text.trim())
    }

    fun assertExpanded() {
        Assert.assertTrue("Expected Expanded", isOpen())
    }

    fun assertCollapsed() {
        Assert.assertFalse("Expected Collapsed", isOpen())
    }

    private fun isOpen(): Boolean {
        var isOpen = false
        expander.actOnTheElement { element ->
            val attribute = element.getAttribute("open")
            if (attribute != null) {
                isOpen = attribute == "true"
            }
        }
        return isOpen
    }

    fun assertNotPresent() {
        expander.assertElementNotPresent()
    }
}