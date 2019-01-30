package pages.organDonation

import models.KeyValuePair
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

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

    private fun assertText(expectedText: String) {
        HybridPageElement(
                "$containerXPath//span",
                page = page,
                helpfulName = "Explanatory text")
                .withText(expectedText)
                .assertSingleElementPresent()
                .assertIsVisible()
    }

    fun assertDecisionIsSome(organsToDonate: ArrayList<KeyValuePair<String, Boolean>>) {
        assertText("Specific organs and tissue")

        val listOfToDonate = arrayListOf<String>()
        val listOfNotToDonate = arrayListOf<String>()

        organsToDonate.forEach { organ ->
            if (organ.value) listOfToDonate.add(organ.key)
            else listOfNotToDonate.add(organ.key)
        }

        assertPair("You have chosen to donate:", listOfToDonate)
        assertPair("You have chosen not to donate:", listOfNotToDonate)
    }

    private fun assertPair(expectedKey: String, expectedValues: ArrayList<String>) {
        val actualValues = HybridPageElement(
                "//h4[text()='$expectedKey']/following-sibling::ul/li",
                page = page,
                helpfulName = "label '$expectedKey'").elements.map { element -> element.text }

        val message = "Expected list of options. " +
                "Expected: ${expectedValues.joinToString()}. " +
                "Actual: ${actualValues.joinToString()}."
        Assert.assertEquals(message, expectedValues.count(), actualValues.count())
        Assert.assertTrue(message, actualValues.containsAll(expectedValues))
    }
}