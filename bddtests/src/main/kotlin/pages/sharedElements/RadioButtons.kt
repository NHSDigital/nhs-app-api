package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement

class RadioButtons(allButtons: HybridPageElement) {

    private val buttons = allButtons.elements.map { element -> RadioButton(element) }

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
}

class RadioButton(val element : WebElementFacade) {

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

    val title: String = findSingle("./label/div/b", "title").text
    val description: String = findSingle("./label/div/p", "description").text

    fun findSingle(xpath:String, name:String): WebElement {
        val found = element.findElements(By.xpath(xpath))
        Assert.assertEquals("Expected found element for $name", 1, found.count())
        return found.single()
    }
}