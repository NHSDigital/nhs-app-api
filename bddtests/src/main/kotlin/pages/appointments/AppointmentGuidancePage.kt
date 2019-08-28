package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks
import utils.contains

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-guidance")
class AppointmentGuidancePage : HybridPageObject() {

    private val mainXPath = "//*[@id='app']/div/main/div/div/div/div/div/div/div/div"

    private val checkSymptomsTitle = "Get help with symptoms"
    private val checkSymptomsDescription = "Find information about specific conditions"

    private val gpAdminTitle = "Request GP help without an appointment"
    private val gpAdminDescription = "Get sick notes and GP letters or ask about recent tests"

    private val gpAdviceTitle = "Request GP help without an appointment"
    private val gpAdviceDescription = "Consult your GP through an online form. " +
            "Your GP surgery will reply by phone or email."

    private var menuContent = MenuLinksContent(
            title = "More",
            links = arrayOf(
                    Pair(checkSymptomsTitle, checkSymptomsDescription),
                    Pair(gpAdminTitle, gpAdminDescription),
                    Pair(gpAdviceTitle, gpAdviceDescription)),
            containerXPath = mainXPath)

    private val menuLinks by lazy { MenuLinks(this, menuContent) }

    val menuCheckSymptomsButton by lazy { menuLinks.link(checkSymptomsTitle) }

    val gpAdminMenuItem by lazy { menuLinks.link(gpAdminTitle) }

    val gpAdviceMenuItem by lazy { menuLinks.link(gpAdviceTitle) }

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
            webDesktopLocator = "//*[@data-purpose='info']",
            androidLocator = null,
            page = this
    )

    fun checkGuidanceBodyForOnlineConsultations(): Boolean {
        return (menuCheckSymptomsButton.isDisplayed &&
        gpAdminMenuItem.isDisplayed &&
        gpAdviceMenuItem.isDisplayed)
    }


    fun getGuidanceBody(): List<Pair<String, Boolean>> {
        val list = arrayListOf<Pair<String, Boolean>>()
        content.actOnTheElement {
            val listElements = it.thenFindAll("*")
            listElements.forEach { listElement ->
                val guidanceLine = listElement.getTextWithoutUnicodeSuffix()
                val isLineInBold = listElement.tagName == "strong"
                list.add(Pair(guidanceLine, isLineInBold))
            }
        }
        return list
    }

}