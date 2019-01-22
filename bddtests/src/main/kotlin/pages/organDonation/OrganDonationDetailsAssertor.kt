package pages.organDonation

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class OrganDonationDetailsAssertor(title: String, val page: HybridPageObject) {

    private val containerXPath = "//div[h3[text()=\"$title\"]]"

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

    fun assert(expectedText: String): OrganDonationDetailsAssertor {
        HybridPageElement(
                "$containerXPath//p[normalize-space() = \"$expectedText\"]",
                page = page,
                helpfulName = "Explanatory text")
                .assertSingleElementPresent()
                .assertIsVisible()
        return this
    }
}