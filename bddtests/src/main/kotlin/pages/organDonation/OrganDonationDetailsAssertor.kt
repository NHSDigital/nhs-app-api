package pages.organDonation

import mocking.organDonation.models.KeyValuePair
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent

class OrganDonationDetailsAssertor private constructor(
            title: String,
            val page: HybridPageObject,
            val titleStyling : String) {

    val containerXPath = "//div[$titleStyling[text()=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    init {
        container.assertSingleElementPresent().assertIsVisible()
    }

    fun assertPair(expectedValues: Array<KeyValuePair<String, String>>): OrganDonationDetailsAssertor {

        val fields = container.element.findElements(By.xpath(".//h4"))

        val actualValues = fields.map { field ->
            KeyValuePair(field.text,
                    field!!.findElement(By.ByXPath("./following-sibling::p")).text)
        }

        Assert.assertEquals("Expected number of pairs. Expected: '${expectedValues.toList()}', Actual: '$actualValues'",
                expectedValues.count(), actualValues.count())

        expectedValues.forEach { expected ->
            val foundPair = actualValues.first { value -> value.key == expected.key }
            Assert.assertEquals("expected value", expected.key, foundPair.key)
        }
        return this
    }

    fun assert(expectedText: Array<String>): OrganDonationDetailsAssertor {
        val actualText = container.element.findElements(By.xpath(".//p"))
                .map { element -> element.text }.toTypedArray()

        expectedText.forEach { expected ->
            Assert.assertTrue("Expected to contain: '$expected'. Actual: '${actualText.joinToString()}'",
                    actualText.contains(expected))
        }
        return this
    }

    companion object {

        fun withH2Header(title: String, page: HybridPageObject):OrganDonationDetailsAssertor{
            return OrganDonationDetailsAssertor(title, page, "h2")
        }

        fun withH3Header(title: String, page: HybridPageObject):OrganDonationDetailsAssertor{
            return OrganDonationDetailsAssertor(title, page, "h3")
        }
    }
}