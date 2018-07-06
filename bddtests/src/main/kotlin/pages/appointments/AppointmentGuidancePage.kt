package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement

@DefaultUrl("http://localhost:3000/appointments/booking-guidance")
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

    val contentHeader = HybridPageElement(
        browserLocator = "//*[@id='mainDiv']/main/div/h2",
        androidLocator = null,
        page = this
    )

    fun getListHeaders(): List<String> {
        val listHeaders = arrayListOf<String>()
        val listElements = findAllByXpath("//*[@id='mainDiv']/main/div/ol/li")
        listElements.forEach { listElement ->
            val listHeader = findByXpath(listElement, "./strong").text
            listHeaders.add(listHeader)
        }
        return listHeaders
    }

}