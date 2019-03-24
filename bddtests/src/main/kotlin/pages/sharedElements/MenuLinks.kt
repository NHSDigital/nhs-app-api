package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertSingleElementPresent

data class MenuLinksContent(val title:String,
                            val links: Array<Pair<String,String>>,
                            val textOverride:String?=null,
                            val containerXPath:String ="//div[h2[text()='${title}']]",
                            val linkStyling:String ="h2")

open class MenuLinks(private val page : HybridPageObject, val content : MenuLinksContent) {

    private val sections = "${content.containerXPath}//ul/li//a"
    private fun specificLinkXPath(linkTitle: String)
            = "$sections[${content.linkStyling}[contains(text(),'$linkTitle')]]"

    private val container = HybridPageElement(
            webDesktopLocator = content.containerXPath,
            androidLocator = null,
            page = page,
            helpfulName = content.title
    )

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = sections,
                androidLocator = null,
                page = page,
                helpfulName = "ListOfLinks"
        )
    }

    fun assertDisplayedWithText() {
        val section = container.assertSingleElementPresent()
        val text = section.element.findElements(By.ByXPath("./*"))
                .map { element -> element.text }.toTypedArray()
        val expectedText = arrayOf(content.title, content.textOverride)
        Assert.assertArrayEquals("expected values in section", expectedText, text)
    }

    fun assertLinksPresent() {
        assertPresentWithLinks(content.links)
    }

    fun assertPresentWithLinks(expectedLinks: Array<Pair<String, String>>) {
        val links = listOfLinks().elements
        Assert.assertEquals("Number of expected Links",
                expectedLinks.count(),
                links.count())
        expectedLinks.forEach { link ->
            link(link.first).assertSingleElementPresent()
            assertDescription(link.first, link.second)
        }
    }

    fun link(linkTitle: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = specificLinkXPath(linkTitle),
                androidLocator = null,
                iOSLocator = "//*[contains(text(),'$linkTitle')]",
                page = page,
                helpfulName = "$linkTitle Link")
    }

    fun count(): Int {
        Assert.assertEquals("number of links", content.links.count(), listOfLinks().elements.count())
        return content.links.count()
    }

    fun assertNotDisplayed() {
        container.assertElementNotPresent()
    }

    private fun linkDescriptionBody(linkTitle: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "${specificLinkXPath(linkTitle)}//p",
                androidLocator = null,
                page = page,
                helpfulName = "$linkTitle Link")
    }

    private fun assertDescription(title: String, description: String) {
        val body = linkDescriptionBody(title)
        Assert.assertEquals("Description Body",
                description,
                body.assertSingleElementPresent().element.text)
    }
}