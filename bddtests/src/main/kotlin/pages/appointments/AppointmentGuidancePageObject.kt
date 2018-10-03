package pages.appointments

import pages.HybridPageElement
import pages.HybridPageObject

class AppointmentGuidancePageObject(pageObject: HybridPageObject){

    private val guidanceParentXpath = "//*[@data-purpose='info-msg']"
    private val guidanceIconXpathFormat = "$guidanceParentXpath//*[@data-purpose='icon']${pageObject
            .containsTextXpathSubstring}"


    val appointmentSlotGuidance = HybridPageElement(
            browserLocator = guidanceParentXpath,
            androidLocator = "",
            page = pageObject,
            helpfulName = "Appointment Slot Guidance section. "
    )

    val expand = HybridPageElement(
            browserLocator = String.format(guidanceIconXpathFormat, "+"),
            androidLocator = "",
            page = pageObject,
            helpfulName = "Appointment Slot Guidance expand icon. "
    )

    val collapse = HybridPageElement(
            // Note that the character is a true minus sign and not a hyphen
            browserLocator = String.format(guidanceIconXpathFormat, "−"),
            androidLocator = "",
            page = pageObject,
            helpfulName = "Appointment Slot Guidance collapse icon. "
    )

    val label = HybridPageElement(
            browserLocator = "$guidanceParentXpath//h2",
            androidLocator = "",
            page = pageObject,
            helpfulName = "Appointment Slot Guidance section label. "
    )

    val content = HybridPageElement(
            browserLocator = "$guidanceParentXpath//*[@data-purpose='info-content']",
            androidLocator = "",
            page = pageObject,
            helpfulName = "Appointment Slot Guidance content. "
    )
}