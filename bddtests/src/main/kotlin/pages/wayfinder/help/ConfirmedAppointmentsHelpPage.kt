package pages.wayfinder.help

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder/help/confirmed-appointments-help")
open class ConfirmedAppointmentsHelpPage : HybridPageObject() {
    private val confirmedAppointmentsHelpPageMainTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Missing or incorrect confirmed appointments\")]",
        page = this,
        helpfulName = "Missing or incorrect confirmed appointments H1"
    )

    private val confirmedAppointmentsHelpPageTitle1 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val confirmedAppointmentsHelpPageTitle2 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If a change or cancellation is not showing\")]",
        page = this,
        helpfulName = "If a change or cancellation is not showing H2"
    )

    private val confirmedAppointmentsHelpPageText1 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have appointments not shown that " +
                "are in other services. Contact the healthcare provider " +
                "the appointment is booked with.\")]",
        page = this,
        helpfulName = "You may have appointments not shown that are in other services. Contact the " +
                "healthcare provider the appointment is booked with. p"
    )

    private val confirmedAppointmentsHelpPageText2 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have requested to change or cancel a " +
                "booked appointment.\")]",
        page = this,
        helpfulName = "You may have requested to change or cancel a booked appointment. p"
    )

    private val confirmedAppointmentsHelpPageText3 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"Any updates you have made may not be shown until " +
                "the request is approved by the healthcare provider " +
                "the appointment is booked with.\")]",
        page = this,
        helpfulName = "Any updates you have made may not be shown until the request is " +
                "approved by the healthcare provider the appointment is booked with. p"
    )

    fun assertHelpPageIsDisplayed(){
        confirmedAppointmentsHelpPageMainTitle.assertIsVisible()
        confirmedAppointmentsHelpPageTitle1.assertIsVisible()
        confirmedAppointmentsHelpPageTitle2.assertIsVisible()
        confirmedAppointmentsHelpPageText1.assertIsVisible()
        confirmedAppointmentsHelpPageText2.assertIsVisible()
        confirmedAppointmentsHelpPageText3.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}



