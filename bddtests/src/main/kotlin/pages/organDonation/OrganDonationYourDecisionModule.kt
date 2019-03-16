package pages.organDonation

import mocking.data.organDonation.OrganDecisions
import org.junit.Assert
import org.openqa.selenium.By
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
        assertText("No, I do not want to donate my organs")
    }

    fun assertDecisionIsYes() {
        assertText("Yes, I do want to donate my organs")
    }

    fun assertDecisionIsAppointedRepresentative() {
        assertText("I have appointed a representative")
    }

    fun assertDecisionIsWithdrawn() {
        assertText("Withdraw my decision from the register")
    }

    private fun assertText(expectedText: String) {
        HybridPageElement(
                "$containerXPath//span",
                page = page,
                helpfulName = "Explanatory text")
                .withText(expectedText)
                .assertSingleElementPresent()
                .assertIsVisible()
    }

    fun assertDecisionIsSome(organsToDonate: OrganDecisions) {

        assertText("Specific organs and tissue")

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
        val actualValues = organSection.element
                .findElements(By.xpath("./following-sibling::ul/li"))
                .map { element -> element.text }

        val message = "Expected list of options. " +
                "Expected: ${expectedValues.joinToString()}. " +
                "Actual: ${actualValues.joinToString()}."
        Assert.assertEquals(message, expectedValues.count(), actualValues.count())
        Assert.assertTrue(message, actualValues.containsAll(expectedValues))
    }

    private fun organSection(expectedKey: String): HybridPageElement {
        return HybridPageElement(
                "//h4[text()='$expectedKey']",
                page = page,
                helpfulName = "label '$expectedKey'")
    }
}
