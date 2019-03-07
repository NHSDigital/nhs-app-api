package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class RadioButtons private constructor(private val page: HybridPageObject, private val locator: String) {

    private val buttons by lazy {   getElements(page, locator).map { element -> RadioButton(element)}}

    fun assertAreEqual(expectedOptions: List<Pair<String, String>>) {
        val expectedTitles = expectedOptions.map { option -> option.first }
        val actualTitles = buttons.map { option -> option.title }
        val message = "Expected list of options. " +
                "Expected: ${expectedTitles.joinToString()}. " +
                "Actual: ${buttons.joinToString { option -> option.title }}."
        Assert.assertEquals(message, expectedTitles.count(), actualTitles.count())
        Assert.assertTrue(message, actualTitles.containsAll(expectedTitles))

        expectedOptions.forEach { expected ->
            val actual = buttons.first { button -> button.title == expected.first }
            if (!expected.second.isEmpty())
                Assert.assertEquals("Expected description for button '${actual.title}'",
                        expected.second, actual.description)
        }
    }

    private fun getElements(page: HybridPageObject, locator: String):
            List<WebElementFacade> {
        return HybridPageElement(
                locator,
                locator,
                page = page,
                helpfulName = "Radio Buttons").elements
    }

    fun button(title: String): RadioButton {
        return buttons.first { button -> button.title == title }
    }

    fun assertSelected(title: String) {
        val selectedRadioButtons = findAllSelected(page, locator)
        Assert.assertEquals("Expected Selected Buttons", 1, selectedRadioButtons.count())
        Assert.assertEquals("Expected Selected Buttons", title,
                RadioButton(selectedRadioButtons.single()).title)
    }

    fun assertAllUnselected() {
        val selectedRadioButtons = findAllSelected(page, locator)
        Assert.assertEquals("Expected Selected Buttons", 0, selectedRadioButtons.count())
    }

    companion object {
        fun create(page: HybridPageObject, locator: String = defaultRadioButtonXPath): RadioButtons {
            return RadioButtons(page, locator)
        }

        fun assertAllOnPageUnselected(page: HybridPageObject) {
            val selectedRadioButtons = findAllSelected(page, defaultRadioButtonXPath)
            Assert.assertEquals("Expected Selected Buttons", 0, selectedRadioButtons.count())
        }

        private fun findAllSelected(page: HybridPageObject, locator: String)
                : List<WebElementFacade> {
            return HybridPageElement(
                    "$locator$selectedIndicatorXPath",
                    "$locator$selectedIndicatorXPath",
                    page = page,
                    helpfulName = "Radio Buttons").elements
        }

        private const val selectedIndicatorXPath = "[div[div]]"
        private const val defaultRadioButtonXPath = "//label[input]"
    }
}

class RadioButton(private val element : WebElementFacade) {

    private val allTextElements = element.findElements(
            By.xpath("./descendant::*[text()]")).map{e->e.text}

    val title: String = allTextElements.first()
    val description: String by lazy { allTextElements[1] }

    fun select() {
        element.click()
    }
}