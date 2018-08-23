package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class AvailableAppointmentFilter(label:String, helpfulName:String, pageObject: HybridPageObject){

    private val byIdXpathFormat = "//*[@id='%s']"

    private val filter = HybridPageElement(
            browserLocator = String.format(byIdXpathFormat, label),
            androidLocator = null,
            page = pageObject,
            helpfulName = helpfulName
    )

    fun selectByText(text: String) {
        filter.element.selectByVisibleText<WebElementFacade>(text)
    }

    fun getSelectedValue(): String {
        return filter.element.selectedVisibleTextValue.trim()
    }

    fun getFilterContents(): ArrayList<String> {
        val optionElements =filter.element.findElements(By.xpath("//option"))
        val optionsAsStrings = arrayListOf<String>()
        for (option in optionElements) {
            optionsAsStrings.add(option.text.trim())
        }
        return optionsAsStrings
    }

    fun assertNotPresent() {
        filter.assertElementNotPresent()
    }
}