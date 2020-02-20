package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/")
open class AppointmentHubPage : HybridPageObject() {

    private val gpAppointmentsTitle = "GP surgery appointments"
    private val gpAppointmentsDescription =
            "View and manage appointments at your surgery"

    var content = LinksWithDescriptionsContent(
            linkBlockTitle = "Appointments",
            containerXPath = "//div[@class='nhsuk-grid-column-full']//div",
            linkStyling = "h2")
            .addLink(gpAppointmentsTitle, gpAppointmentsDescription)

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

    fun assertAppointmentsHubIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun assertLinksPresent() {
        links.assertLinksPresent(true)
    }
}
