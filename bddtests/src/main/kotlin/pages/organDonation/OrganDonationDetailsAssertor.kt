package pages.organDonation

import net.serenitybdd.core.pages.WebElementFacade
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

    fun assertPair(expectedKey: String, expectedValue: String): OrganDonationDetailsAssertor {
        val field = HybridPageElement(
                "$containerXPath//h4",
                page = page,
                helpfulName = "label '$expectedKey'").withText(expectedKey)
                .assertSingleElementPresent()
                .assertIsVisible()

        val actualValue = field.element.find<WebElementFacade>(By.ByXPath("./following-sibling::p")).text
        Assert.assertEquals("Value for '$expectedKey'", actualValue, expectedValue)
        return this
    }

    fun assert(expectedText: Array<String>): OrganDonationDetailsAssertor {
        val actualText =HybridPageElement(
                "$containerXPath//p",
                page = page).elements.map { element -> element.text }.toTypedArray()

        expectedText.forEach { expected->
            Assert.assertTrue("Expected to contain: '$expected'. Actual: '${actualText.joinToString ()}'",
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