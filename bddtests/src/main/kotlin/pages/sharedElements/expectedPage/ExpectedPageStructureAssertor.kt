package pages.sharedElements.expectedPage

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import kotlin.streams.toList

class ExpectedPageStructureAssertor {
    private val tagsToAssert = listOf("h1", "h2", "h3", "h4", "li", "p", "span", "button",
            "details", "summary", "label", "select", "option")
    private val tagsToExclude = arrayListOf(
            "div", "br", "ul", "ol", "a", "nav", "svg", "path", "form",
            "input", "hr", "b", "g", "circle", "strong", "i")
    private val knownTags = tagsToAssert + tagsToExclude

    fun assert(page: HybridPageObject, expectedPage: MutableList<ExpectedPageStructureElement>) {
        val containerPath = pathToElementContainer(expectedPage[0])
        val parsedPage = ParsedPage.parse(page, containerPath)
        assert(parsedPage, expectedPage)
    }

    fun assert(parsedPage: ParsedPage, expectedPage: MutableList<ExpectedPageStructureElement>) {
        val actualElementsToAssert = getTagsToAssert(parsedPage.parsedElements)
        val expectedVersusActual = expectedPage.zip(actualElementsToAssert)
        for (pair in expectedVersusActual) {
            pair.first.assert(pair.second)
        }
        if (expectedPage.size != actualElementsToAssert.size) {
            assertSize(expectedVersusActual, expectedPage, actualElementsToAssert)
        }
    }

    fun assertElementNotPresent(page: HybridPageObject, expectedPage: MutableList<ExpectedPageStructureElement>) {
        val containerPath = pathToElementContainer(expectedPage[0])
        HybridPageElement(containerPath, page = page).assertElementNotPresent()
    }

    private fun assertSize(zipped: List<Pair<ExpectedPageStructureElement, ParsedPageElement>>,
                           expectedPage: MutableList<ExpectedPageStructureElement>,
                           actualElementsToAssert: List<ParsedPageElement>) {
        if (zipped.size < expectedPage.size) {
            val spare = expectedPage.elementAt(zipped.size)
            assertIncorrectNumber(expectedPage.size, actualElementsToAssert.size, spare.tag, spare.content)
        }
        if (zipped.size < actualElementsToAssert.size) {
            val spare = actualElementsToAssert.elementAt(zipped.size)
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

    private fun getTagsToAssert(elements: List<ParsedPageElement>): List<ParsedPageElement> {
        if (elements.any { element -> !knownTags.contains(element.tag) }) {
            val notExpected = elements.filter { element -> !knownTags.contains(element.tag) }
                    .map { element -> "'${element.tag}' containing '${element.content}'" }.toList()
                    .joinToString ()
            Assert.fail("Html tags must be explicitly excluded. Unexpected tags found: $notExpected")
        }
        return elements.stream()
                .filter { element -> tagsToAssert.contains(element.tag) && element.content.isNotEmpty() }.toList()
    }

    private fun pathToElementContainer(element: ExpectedPageStructureElement): String {
        return "//div[${element.tag}[normalize-space(text())=\"${element.content}\"]]"
    }
}
