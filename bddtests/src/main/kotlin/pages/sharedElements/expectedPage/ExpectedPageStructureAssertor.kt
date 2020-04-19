package pages.sharedElements.expectedPage

import ExpectedPageStructureElement
import ParsedPageElement
import org.junit.Assert
import pages.HybridPageObject
import pages.avoidChromeWebDriverServiceCrash
import kotlin.streams.toList

class ExpectedPageStructureAssertor {
    private val tagsToAssert  = arrayListOf("h1", "h2", "h3", "h4", "li", "p", "span", "button")
    private val tagsToExclude = arrayListOf(
            "div", "br", "ul", "ol", "a", "nav", "svg", "path", "form", "input", "hr", "b")
    private val knownTags = tagsToAssert  + tagsToExclude

    fun assert(page: HybridPageObject, expectedPage: MutableList<ExpectedPageStructureElement>) {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        page.avoidChromeWebDriverServiceCrash()
        val firstElement = expectedPage[0]
        val containerPath = "//div[${firstElement.tag}[normalize-space(text())=\"${firstElement.content}\"]]"
        val parsedPage = ParsedPage.parse(page, containerPath)
        assert(parsedPage, expectedPage)
    }

    fun assert(parsedPage: ParsedPage, expectedPage: MutableList<ExpectedPageStructureElement>) {
        val actualElementsToAssert =
                parsedPage.parsedElements.stream().filter { element -> isTagToAssert(element) }.toList()
        val expectedVersusActual = expectedPage.zip(actualElementsToAssert)
        for (pair in expectedVersusActual) {
            pair.first.assert(pair.second)
        }
        if (expectedPage.size != actualElementsToAssert.size) {
            assertSize(expectedVersusActual, expectedPage, actualElementsToAssert)
        }
    }

    private fun assertSize(zipped: List<Pair<ExpectedPageStructureElement, ParsedPageElement>>,
                           expectedPage: MutableList<ExpectedPageStructureElement>,
                           actualElementsToAssert: List<ParsedPageElement>) {
        if (zipped.size < expectedPage.size) {
            val spare = expectedPage.elementAt(expectedPage.size)
            assertIncorrectNumber(expectedPage.size, actualElementsToAssert.size, spare.tag, spare.content)
        }
        if (zipped.size < actualElementsToAssert.size) {
            val spare = actualElementsToAssert.elementAt(expectedPage.size)
            assertIncorrectNumber(expectedPage.size, actualElementsToAssert.size, spare.tag, spare.content)
        }
    }

    private fun assertIncorrectNumber(expectedSize: Int,
                                      actualSize: Int,
                                      unexpectedTag: String,
                                      unexpectedContent: String) {
        Assert.fail(
                "Expected $expectedSize elements but found $actualSize elements." +
                        "\n" +
                        "First unaccounted element '$unexpectedTag', '$unexpectedContent'")
    }

    private fun isTagToAssert(element: ParsedPageElement): Boolean {
        if (!knownTags.contains(element.tag)) {
            Assert.fail("Html tags must be explicitly excluded. " +
                    "Unexpected tag found: '${element.tag}' containing '${element.content}'")
        }
        return tagsToAssert.contains(element.tag) && element.content.isNotEmpty()
    }
}