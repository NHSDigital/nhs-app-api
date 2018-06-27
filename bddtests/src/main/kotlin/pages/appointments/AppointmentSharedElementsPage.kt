package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.By
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.WebElement
import pages.HybridPageObject

open class AppointmentSharedElementsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun convertToSlotObject(parentContainer: WebElementFacade, parentToSlotDivRelativePath: String = "", isMyAppointmentSlot: Boolean = false): Slot {
        val slot = Slot()
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) "./" else "$parentToSlotDivRelativePath/"
        slot.time = findByXpath(parentContainer, relativePath + "h4").text
        slot.session = findByXpath(parentContainer, relativePath + "p[@aria-label='session name']").text
        slot.date = findByXpath(parentContainer, relativePath + "h5").text

        val locationElement = findByXpath(parentContainer, relativePath + "p[@aria-label='location']")
        slot.location = getSlotChildElementDisplayingText(locationElement)

        retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath, isMyAppointmentSlot)
        return slot
    }

    fun clickOnButton(button: String) {
        val element: WebElementFacade = findByXpath("//button[contains(text(),'$button')]")
        scrollToTheElement(element)
        element.click()
    }

    private fun retrieveClinicianAndAddToSlot(slot: Slot, parentContainer: WebElementFacade, relativePath: String, isMyAppointmentSlot: Boolean) {
        if (isMyAppointmentSlot) {
            val clinicianElements = findAllByXpath(parentContainer, relativePath + "p[@aria-label='clinicians']")
            clinicianElements.forEach { clinicianElement ->
                val clinicianDisplayName = getSlotChildElementDisplayingText(clinicianElement)
                slot.clinician.add(clinicianDisplayName)
            }
        } else {
            val clinicians: List<WebElement> = findAllByXpath(parentContainer, relativePath + "ul/li")
            clinicians.forEach { clinician ->
                val clinicianDisplayName = getSlotChildElementDisplayingText(clinician)
                slot.clinician.add(clinicianDisplayName)
            }
        }
    }

    private fun getSlotChildElementDisplayingText(childElement: WebElement): String {
        val childElementText = childElement.text
        return try {
            val svg = childElement.findElement(By.tagName("svg"))
            childElementText.replace(svg.text, "").trim()
        } catch (e: Exception) {
            childElementText.trim()
        }
    }

}