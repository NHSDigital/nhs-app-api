package pages.organDonation

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement

class RadioButtons(allButtons: HybridPageElement) {

    private val buttons = allButtons.elements.map { element-> RadioButton(element) }

    fun assertAreEqual(expectedOptions: List<Pair<String, String>>) {
        val expectedTitles = expectedOptions.map { option -> option.first }
        val actualTitles = buttons.map { option -> option.title }
        val message = "Expected list of options. " +
                "Expected: ${expectedTitles.joinToString()}. " +
                "Actual: ${buttons.map { option -> option.title }.joinToString()}."
        Assert.assertEquals(message, expectedTitles.count(), actualTitles.count())
        Assert.assertTrue(message, actualTitles.containsAll(expectedTitles))

        expectedOptions.forEach { expected ->
            val actual = buttons.first { button -> button.title == expected.first }
            Assert.assertEquals("Expected description for button '${actual.title}'",
                    expected.second, actual.description)
        }
    }

    fun button(title:String): RadioButton {
        return buttons.first{button->button.title == title}
    }

    fun assertSelected(title:String) {
        buttons.forEach { button ->
            if (button.title == title) {
                button.assertSelected()
            } else {
                button.assertNotSelected()
            }
        }
    }
}

class RadioButton(val element : WebElementFacade) {

    fun assertSelected() {
        Assert.assertTrue("$title should be selected but is not", selected)
    }

    fun assertNotSelected() {
        Assert.assertFalse("$title should not be selected but is", selected)
    }

    fun select() {
        element.click()
    }

    private val selected: Boolean = element.find<WebElementFacade>(By.xpath("./input")).getAttribute("value") == "true"

    val title: String = element.find<WebElementFacade>(By.xpath("./b")).text
    val description: String = element.find<WebElementFacade>(By.xpath("./p")).text
}