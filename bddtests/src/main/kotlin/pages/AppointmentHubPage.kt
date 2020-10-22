package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/")
open class AppointmentHubPage : HybridPageObject() {

    private val gpAppointmentsTitle = "GP surgery appointments"
    private val gpAppointmentsDescription = "View and manage appointments at your surgery"

    private val engageAdminTitle = "Additional GP services"
    private val engageAdminDescription = "Get sick notes and GP letters or ask about recent tests"

    private val olcAdminHelpTitle = "Additional GP services"
    private val olcAdminHelpDescription = "Get sick notes and GP letters or ask about recent tests"

    var content = LinksWithDescriptionsContent(
            linkBlockTitle = "Appointments",
            containerXPath = "//div[@class='nhsuk-grid-column-full']",
            linkStyling = "h2")
            .addLink(gpAppointmentsTitle, gpAppointmentsDescription)
            .addLink(engageAdminTitle, engageAdminDescription)
            .addLink(olcAdminHelpTitle, olcAdminHelpDescription)

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"Appointments\")]",
            androidLocator = null,
            page = this,
            helpfulName = "Appointments Hub Title"
    )

    val links by lazy { LinksElement(this, content) }

    val btnGPAppointmentsLinksWithDescriptionsContent by lazy {
        links.link(gpAppointmentsTitle)
    }

    val btnEngageAdminLinksWithDescriptionsContent by lazy {
        links.link(engageAdminTitle)
    }

    val btnOlcAdminHelpLinkWithDescriptionsContent by lazy {
        links.link(olcAdminHelpTitle)
    }

    val hospitalAppointmentsLink by lazy {
        links.link("Hospital and other appointments")
    }

    fun assertAppointmentsHubIsDisplayed() {
        pageTitle.assertIsVisible()
    }
}
