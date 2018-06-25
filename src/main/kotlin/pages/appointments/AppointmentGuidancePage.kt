package pages.appointments

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject

@DefaultUrl("http://localhost:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(id = "btn_check_symptoms")
    lateinit var checkSymptomsButtonElement: WebElementFacade

    @FindBy(id = "btn_appointment")
    lateinit var bookButtonElement: WebElementFacade

    @FindBy(xpath = "//*[@id='mainDiv']/main/div/h2")
    lateinit var contentHeaderElement: WebElementFacade

    fun clickSymptomsCheckerButton() {
        clickTheButton(checkSymptomsButtonElement)
    }

    fun clickBookButton() {
        clickTheButton(bookButtonElement)
    }

    fun getListHeaders(): List<String> {
        val listHeaders = arrayListOf<String>()
        val listElements = findAllByXpath("//*[@id='mainDiv']/main/div/ol/li")
        listElements.forEach { listElement ->
            val listHeader = findByXpath(listElement, "./strong").text
            listHeaders.add(listHeader)
        }
        return listHeaders
    }

    private fun clickTheButton(button: WebElementFacade) {
        button.waitUntilClickable<WebElementFacade>()
        scrollToTheElement(button)
        button.click()
    }

}