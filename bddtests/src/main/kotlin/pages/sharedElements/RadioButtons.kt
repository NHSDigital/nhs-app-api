package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class RadioButtons private constructor(private val buttons: List<RadioButton>) {

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
        buttons.forEach { button ->
            if (button.title == title) {
                button.assertSelected()
            } else {
                button.assertNotSelected()
            }
        }
    }

    fun assertAllUnselected() {
        buttons.forEach { button ->
            button.assertNotSelected()
        }
    }

    companion object {
        fun create(page: HybridPageObject, locator: String? = null): RadioButtons {
            return RadioButtons(getElements(page, locator).map { element -> RadioButton(element) })
        }

        private fun getElements(page: HybridPageObject, locator: String?):
                List<WebElementFacade> {
            return HybridPageElement(
                    locator ?: "//label[input][div]",
                    locator ?: "//label[input][div]",
                    page = page,
                    helpfulName = "Radio Buttons").elements
        }
    }
}

class RadioButton(private val element : WebElementFacade) {

    private val allTextElements = element.findElements(
            By.xpath("./descendant::*[text()]")).map{e->e.text}

    val title: String = allTextElements.first()
    val description: String by lazy { allTextElements[1] }

    fun assertSelected() {
        Assert.assertEquals("$title should be selected but is not", 1, selectedIndicator().count())
    }

    fun assertNotSelected() {
        Assert.assertEquals("$title should not be selected but is", 0, selectedIndicator().count())
    }

    fun select() {
        element.click()
    }

    private fun selectedIndicator() = element.findElements(By.xpath("./div/div"))
}