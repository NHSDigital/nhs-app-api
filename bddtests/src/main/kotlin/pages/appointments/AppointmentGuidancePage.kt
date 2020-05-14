package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject() {

    private val mainXPath = "//*[@id='app']/div/main/div/div/div/div/div/div/div/div"

    private val checkSymptomsTitle = "Get help with symptoms"
    private val checkSymptomsDescription = "Find information about specific conditions"

    private val gpAdminTitle = "Additional GP services"
    private val gpAdminDescription = "Get sick notes and GP letters or ask about recent tests"

    private val gpAdviceTitle = "Ask your GP for advice"
    private val gpAdviceDescription = "Consult your GP through an online form. " +
            "Your GP surgery will reply by phone or email."

    private var menuContent = LinksWithDescriptionsContent(
            linkBlockTitle = "More",
            containerXPath = mainXPath,
            linkStyling = "h2")
            .addLink(checkSymptomsTitle, checkSymptomsDescription)
            .addLink(gpAdminTitle, gpAdminDescription)
            .addLink(gpAdviceTitle, gpAdviceDescription)

    private val menuLinks by lazy { LinksElement(this, menuContent) }

    val menuCheckSymptomsButton by lazy { menuLinks.link(checkSymptomsTitle) }

    val gpAdminMenuItem by lazy { menuLinks.link(gpAdminTitle) }

    val gpAdviceMenuItem by lazy { menuLinks.link(gpAdviceTitle) }

    val bookButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_appointment']",
            androidLocator = null,
            page = this
    )

    fun checkGuidanceBodyForOnlineConsultations() {
        menuCheckSymptomsButton.assertSingleElementPresent()
        gpAdminMenuItem.assertSingleElementPresent()
        gpAdviceMenuItem.assertSingleElementPresent()
    }
}