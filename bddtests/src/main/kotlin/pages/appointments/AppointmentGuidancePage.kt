package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageObject
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val checkSymptomsButton = HybridPageElement(
            browserLocator = "//*[@id='btn_check_symptoms']",
            androidLocator = null,
            page = this
    )

    val bookButton = HybridPageElement(
            browserLocator = "//*[@id='btn_appointment']",
            androidLocator = null,
            page = this
    )

    val content = HybridPageElement(
            browserLocator = "//*[@id='app']/main/div",
            androidLocator = null,
            page = this
    )

    fun isSubHeaderTextEqualTo(text: String, elementWasStale: Boolean = false): Boolean {
        return try {
            content.element.findBy<WebElementFacade>("//h2[text()='$text']")
            true
        } catch (e: StaleElementReferenceException) {
            if (!elementWasStale) {
                isSubHeaderTextEqualTo(text, true)
            } else {
                false
            }
        } catch (e: NoSuchElementException) {
            false
        }
    }

    fun getGuidanceBody(): List<Pair<String, Boolean>> {
        val list = arrayListOf<Pair<String, Boolean>>()
        val listElements = content.element.thenFindAll("div/*")
        listElements.forEach { listElement ->
            val guidanceLine = listElement.text
            val isLineInBold = listElement.tagName == "strong"
            list.add(Pair(guidanceLine, isLineInBold))
        }
        return list
    }

}