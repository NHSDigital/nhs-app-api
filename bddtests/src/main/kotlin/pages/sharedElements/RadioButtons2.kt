package pages.sharedElements

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement

class RadioButtons2(allButtons: HybridPageElement) {

    private val buttons = allButtons.elements.map { element -> RadioButton2(element) }

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
            if(!expected.second.isEmpty())
            Assert.assertEquals("Expected description for button '${actual.title}'",
                    expected.second, actual.description)
        }
    }

    fun button(title: String): RadioButton2 {
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

class RadioButton2(private val element : WebElementFacade) {

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


    val title: String = element.find<WebElementFacade>(By.xpath("./label/span")).text
    val description: String by lazy { element.find<WebElementFacade>(By.xpath("./label/p")).text }
}