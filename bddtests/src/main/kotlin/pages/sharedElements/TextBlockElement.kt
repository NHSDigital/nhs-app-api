package pages.sharedElements

import mocking.organDonation.models.KeyValuePair
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent

class TextBlockElement private constructor(
            title: String,
            page: HybridPageObject,
            titleStyling : String) {

    private val containerXPath = "//div[$titleStyling[normalize-space(text())=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    fun assertElementNotPresent(){
        container.assertElementNotPresent()
    }

    fun assertPair(expectedValues: Array<KeyValuePair<String, String>>): TextBlockElement {

        container.actOnTheElement {
            val fields = it.findElements(By.xpath(".//h4"))

            val actualValues = fields.map { field ->
                KeyValuePair(field.text,
                        field!!.findElement(By.ByXPath("./following-sibling::p")).text)
            }

            Assert.assertEquals(
                    "Expected number of pairs. Expected: '${expectedValues.toList()}', Actual: '$actualValues'",
                    expectedValues.count(), actualValues.count())

            expectedValues.forEach { expected ->
                val foundPair = actualValues.first { value -> value.key == expected.key }
                Assert.assertEquals("expected value", expected.key, foundPair.key)
            }
        }
        return this
    }

    fun assert(vararg expectedTexts: String): TextBlockElement {
        return assertInternal(".//p", expectedTexts)
    }

    fun assertList(vararg expectedValues : String): TextBlockElement{
        return assertInternal("./ul/li", expectedValues)
    }

    private fun assertInternal(locator: String, expectedText: Array<out String>): TextBlockElement {
        container.actOnTheElement {
            val actualText = it.findElements(By.xpath(locator))
                    .map { element -> element.text }.toTypedArray()

            expectedText.forEach { expected ->
                Assert.assertTrue("Expected to contain: '$expected'. Actual: '${actualText.joinToString()}'",
                        actualText.contains(expected))
            }
        }
        return this
    }

    companion object {

        fun withH2Header(title: String, page: HybridPageObject):TextBlockElement{
            return TextBlockElement(title, page, "h2")
        }

        fun withH3Header(title: String, page: HybridPageObject):TextBlockElement{
            return TextBlockElement(title, page, "h3")
        }
    }
}