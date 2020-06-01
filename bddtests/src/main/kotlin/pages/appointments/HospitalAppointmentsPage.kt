package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.sharedElements.LinksWithDescriptionsContent
import pages.sharedElements.LinksElement

@DefaultUrl("http://web.local.bitraft.io:3000/hospital-appointments/")
open class HospitalAppointmentsPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this,
            helpfulName = "Hospital Appointments Title"
    ).withText("Hospital and other appointments", normalised = true)

    private val hospitalAppointmentsTitle = "Book or cancel your referral appointment"
    private val hospitalAppointmentsDescription =
            "If you've had a referral, you can book or cancel your first appointment here"
    private val viewCieAppointmentsTitle = "View appointments"
    private val viewCieAppointmentsDescription = "See your upcoming and past hospital or other appointments"
    private val viewPkbAppointmentsTitle = "View appointments"
    private val viewPkbAppointmentsDescription = "See your upcoming and past hospital or other appointments"

    private val content = LinksWithDescriptionsContent(
            linkBlockTitle = "Appointments",
            containerXPath = "//div[@id='maincontent']//div",
            linkStyling = "h2")
            .addLink(hospitalAppointmentsTitle, hospitalAppointmentsDescription)

    private val links by lazy { LinksElement(this, content) }
    val btnCieAppointments by lazy { links.link(viewCieAppointmentsTitle,
            viewCieAppointmentsDescription) }
    val btnPkbAppointments by lazy { links.link(viewPkbAppointmentsTitle,
            viewPkbAppointmentsDescription) }

    fun assertPageTitleIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun assertLinksDisplayed() {
        links.assertLinksPresent(true)
    }

    fun assertCieViewAppointmentsIsDisplayed() {
        btnPkbAppointments.assertSingleElementPresent()
    }
    fun assertPkbViewAppointmentsIsDisplayed() {
        btnPkbAppointments.assertSingleElementPresent()
    }
}
