package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject() {

    private val mainXPath = "//*[@id='app']/div/main/div"

    private val checkSymptomsTitle = "Get help with symptoms"
    private val checkSymptomsDescription = "Find information about specific conditions"

    private var menuContent = MenuLinksContent(
            title = "More",
            links = arrayOf(
                    Pair(checkSymptomsTitle, checkSymptomsDescription)),
            containerXPath = mainXPath)

    private val menuLinks by lazy { MenuLinks(this, menuContent) }

    val menuCheckSymptomsButton by lazy { menuLinks.link(checkSymptomsTitle) }

    val bookButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_appointment']",
            androidLocator = null,
            page = this
    )
}