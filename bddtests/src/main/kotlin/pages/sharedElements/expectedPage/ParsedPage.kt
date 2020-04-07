package pages.sharedElements.expectedPage

import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject

class ParsedPage private constructor(val parsedElements: List<ParsedPageElement>) {

    companion object {

        fun parse(page: HybridPageObject, containerPath: String): ParsedPage {
            val parsedElements = mutableListOf<ParsedPageElement>()
            val container = findContainer(page, containerPath)
            container.actOnTheElement { elementToParse ->
                parsedElements.addAll(elementToParse.findElements<WebElement>(By.xpath(".//*"))
                        .map { element -> ParsedPageElement(element) })
            }
            return ParsedPage(parsedElements)
        }

        private fun findContainer(page: HybridPageObject, containerPath: String): HybridPageElement {
            return HybridPageElement(
                    containerPath,
                    page = page)
        }
    }
}