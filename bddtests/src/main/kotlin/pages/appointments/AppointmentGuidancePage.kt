package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject() {

    private val mainXPath = "//*[@id='app']/main"

    val checkSymptomsButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_check_symptoms']",
            androidLocator = null,
            page = this
    )

    val bookButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_appointment']",
            androidLocator = null,
            page = this
    )

    private val main = HybridPageElement(
            webDesktopLocator = mainXPath,
            androidLocator = null,
            page = this
    )

    private val content = HybridPageElement(
            webDesktopLocator = "$mainXPath/div/div[@data-purpose='info']",
            androidLocator = null,
            page = this
    )

    fun isSubHeaderTextEqualTo(text: String, elementWasStale: Boolean = false): Boolean {
        return try {
            main.element.findBy<WebElementFacade>("//*[@id='guidance_sub_header' and contains(text(), '$text')]")
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
        val listElements = content.element.thenFindAll("*")
        listElements.forEach { listElement ->
            val guidanceLine = listElement.text
            val isLineInBold = listElement.tagName == "strong"
            list.add(Pair(guidanceLine, isLineInBold))
        }
        return list
    }

    fun assertNativeElementsLoaded(){
        if(onMobile()) {
            shouldBeVisibleOnNative(bookButton)
        }
    }

}