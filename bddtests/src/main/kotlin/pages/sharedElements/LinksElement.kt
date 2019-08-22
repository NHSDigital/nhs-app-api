package pages.sharedElements

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertSingleElementPresent
import pages.text

open class LinksElement(private val page : HybridPageObject, val content : ILinksContent) {

    private val sections = "${content.containerXPath}//ul/li//a"

    private val container by lazy {
        HybridPageElement(
                webDesktopLocator = content.containerXPath,
                androidLocator = null,
                page = page,
                helpfulName = content.linkBlockTitle
        )
    }

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = sections,
                androidLocator = null,
                page = page,
                helpfulName = "ListOfLinks"
        )
    }

    fun assertLinksPresent() {
        assertPresentWithLinks(content.links)
    }

    fun assertReducedSetOfLinksPresent(vararg titles: String) {
        if (!titles.any()) {
            Assert.fail("Some titles required to filter")
        }
        val links = content.links
                .filter { link -> titles.any { title -> title == link.title } }.toList()
        assertPresentWithLinks(links)
    }

    fun link(linkTitle: String): HybridPageElement {
        val linkPath = content.specificLinkXPath(linkTitle)
        return HybridPageElement(
                webDesktopLocator = linkPath,
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
                webDesktopLocator = "${content.specificLinkXPath(linkTitle)}//p",
                androidLocator = null,
                page = page,
                helpfulName = "$linkTitle Link")
    }

    private fun assertDescription(title: String, description: String?) {
        val body = linkDescriptionBody(title)
        if (description != null) {
            Assert.assertEquals("Description Body",
                    description,
                    body.assertSingleElementPresent().text)
        } else {
            if (body.elements.count() != 0) {
                Assert.assertEquals("Description Body should be empty",
                        "",
                        body.textValue)
            }
        }
    }

    private fun assertPresentWithLinks(expectedLinks: List<LinkContent>) {
        val links = listOfLinks().elements
        Assert.assertEquals("Number of expected LinksElement",
                expectedLinks.count(),
                links.count())
        if (!content.hasDescriptionBody) {
            Assert.assertArrayEquals("Expected links",
                    expectedLinks.map { link -> link.title }.toTypedArray(),
                    links.map { link -> link.text }.toTypedArray())
        } else {
            expectedLinks.forEach { link ->
                link(link.title).assertSingleElementPresent()
                assertDescription(link.title, link.description)
            }
        }
    }
}