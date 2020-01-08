package pages.organDonation

import mocking.data.organDonation.OrganDecisions
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.assertSingleElementPresent

class OrganDonationYourDecisionModule(private val page: HybridPageObject) {

    private val title = "Your decision"

    private val containerXPath = "//div[h2[text()=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    init {
        container.assertSingleElementPresent().assertIsVisible()
    }

    fun assertDecisionIsNo() {
        assertText("No I do not want to donate my organs")
    }

    fun assertDecisionIsYes() {
        assertText("Yes I want to donate my organs")
    }

    fun assertDecisionIsAppointedRepresentative() {
        assertText("I have appointed a representative")
    }

    fun assertDecisionIsWithdrawn() {
        assertText("Withdraw my decision from the register")
    }

    private fun assertText(expectedText: String) {
        HybridPageElement(
                "$containerXPath//p[normalize-space(text())='$expectedText']",
                page = page,
                helpfulName = "Explanatory text")
                .assertSingleElementPresent()
                .assertIsVisible()
    }

    fun assertDecisionIsSome(organsToDonate: OrganDecisions) {

        assertText("Yes I want to donate my organs")

        assertPair("You have chosen to donate:", organsToDonate.optIn)
        assertPair("You have chosen not to donate:", organsToDonate.optOut)
        assertPair("We do not have a decision for:", organsToDonate.notStated)
    }

    private fun assertPair(expectedKey: String, expectedValues: ArrayList<String>) {
        val organSection = organSection(expectedKey)

        if (expectedValues.isEmpty()) {
            organSection.assertElementNotPresent()
            return
        }
        organSection(expectedKey).assertIsVisible()
        organSection.actOnTheElement {
            val actualValues =
                    it.findElements<WebElement>(By.xpath("./following-sibling::ul/li"))
                    .map { element -> element.text }

            val message = "Expected list of options. " +
                    "Expected: ${expectedValues.joinToString()}. " +
                    "Actual: ${actualValues.joinToString()}."
            Assert.assertEquals(message, expectedValues.count(), actualValues.count())
            Assert.assertTrue(message, actualValues.containsAll(expectedValues))
        }
    }

    private fun organSection(expectedKey: String): HybridPageElement {
        return HybridPageElement(
                "//h4[normalize-space(text())='$expectedKey']",
                page = page,
                helpfulName = "label '$expectedKey'")
    }
}
