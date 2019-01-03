package pages.appointments

import pages.HybridPageElement
import pages.HybridPageObject

class AppointmentGuidancePageObject(pageObject: HybridPageObject){

    private val guidanceParentXpath = "//*[@data-purpose='info-msg']"
    private val guidanceIconXpathFormat = "$guidanceParentXpath//*[@data-purpose='icon']${pageObject
            .containsTextXpathSubstring}"


    val appointmentSlotGuidance = HybridPageElement(
            webDesktopLocator = guidanceParentXpath,
            androidLocator = null,
            page = pageObject,
            helpfulName = "Appointment Slot Guidance section. "
    )

    val expand = HybridPageElement(
            webDesktopLocator = String.format(guidanceIconXpathFormat, "+"),
            androidLocator = null,
            page = pageObject,
            helpfulName = "Appointment Slot Guidance expand icon. "
    )

    val collapse = HybridPageElement(
            // Note that the character is a true minus sign and not a hyphen
            webDesktopLocator = String.format(guidanceIconXpathFormat, "−"),
            androidLocator = null,
            page = pageObject,
            helpfulName = "Appointment Slot Guidance collapse icon. "
    )

    val label = HybridPageElement(
            webDesktopLocator = "$guidanceParentXpath//h2",
            androidLocator = null,
            page = pageObject,
            helpfulName = "Appointment Slot Guidance section label. "
    )

    val content = HybridPageElement(
            webDesktopLocator = "$guidanceParentXpath//*[@data-purpose='info-content']",
            androidLocator = null,
            page = pageObject,
            helpfulName = "Appointment Slot Guidance content. "
    )
}