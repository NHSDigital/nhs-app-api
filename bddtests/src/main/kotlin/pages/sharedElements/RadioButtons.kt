package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject

class RadioButtons private constructor(private val page: HybridPageObject, private val locator: String) {

    private val buttons by lazy {   getButtons(page, locator) }

    fun assertAreEqual(expectedOptions: ArrayList<String>) {
        val actualTitles = buttons.map { option -> option.title }
        Assert.assertArrayEquals("Expected options",
                expectedOptions.toTypedArray(),
                actualTitles.toTypedArray())
    }

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

    fun button(title: String): RadioButton {
        return buttons.first { button -> button.title == title }
    }

    fun assertSelected(title: String) {
        val selectedRadioButtons = findAllSelected(buttons)
        Assert.assertEquals("Expected Selected Buttons", 1, selectedRadioButtons.count())
        Assert.assertEquals("Expected Selected Buttons", title, selectedRadioButtons.first().title)
    }

    fun assertAllUnselected() {
        val selectedRadioButtons = findAllSelected(buttons)
        Assert.assertEquals("Expected Selected Buttons", 0, selectedRadioButtons.count())
    }

    companion object {
        fun create(page: HybridPageObject, locator: String = defaultRadioButtonXPath): RadioButtons {
            return RadioButtons(page, locator)
        }

        fun assertAllOnPageUnselected(page: HybridPageObject) {
            val buttons = getButtons(page, defaultRadioButtonXPath)
            val selectedRadioButtons = findAllSelected(buttons)
            Assert.assertEquals("Expected Selected Buttons", 0, selectedRadioButtons.count())
        }

        private fun findAllSelected(buttons: List<RadioButton>)
                : List<RadioButton> {
            return buttons.filter { button -> button.isSelected() }
        }

        private fun getButtons(page: HybridPageObject, locator: String)
                : List<RadioButton> {
            return HybridPageElement(
                    locator,
                    locator,
                    page = page,
                    helpfulName = "Radio Buttons").elements.map { element -> RadioButton(element) }
        }

        private const val defaultRadioButtonXPath = "//div[input[@type=\"radio\"]]"
    }
}

class RadioButton(private val element : WebElementFacade) {

    private val allTextElements = element.findElements<WebElement>(
            By.xpath("./descendant::*[text()]")).map{e->e.text}
    private val input = element.findElement<WebElement>(By.tagName("input"))

    val title: String by lazy { allTextElements[0] }
    val description: String by lazy { allTextElements[1] }

    fun select() {
        input.click()
    }

    fun isSelected(): Boolean {
        return input.isSelected
    }
}
