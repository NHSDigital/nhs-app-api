package pages.wayfinder.help

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder/help/referrals-or-appointments-help")
open class ReferralsOrAppointmentsHelpPage : HybridPageObject() {
    private val missingOrIncorrectReferralsOrAppointmentsHelpPageMainTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Missing or incorrect referrals or appointments\")]",
        page = this,
        helpfulName = "Missing or incorrect referrals or appointments H1"
    )

    private val missingOrIncorrectReferralsOrAppointmentsHelpPageTitle1 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val missingOrIncorrectReferralsOrAppointmentsHelpPageText1 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have referrals or appointments not shown " +
                "that are in other services. Contact the relevant organisation or healthcare provider.\")]",
        page = this,
        helpfulName = "You may have referrals or appointments not shown that are in other services. " +
                "Contact the relevant organisation or healthcare provider. p"
    )

    private val missingOrIncorrectReferralsOrAppointmentsHelpPageTitle2 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If a change or cancellation is not showing\")]",
        page = this,
        helpfulName = "If a change or cancellation is not showing H2"
    )

    private val missingOrIncorrectReferralsOrAppointmentsHelpPageText2 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have requested to change or cancel a referral " +
                "or appointment.\")]",
        page = this,
        helpfulName = "You may have requested to change or cancel a referral or appointment. p"
    )

    private val missingOrIncorrectReferralsOrAppointmentsHelpPageText3 = HybridPageElement(
        webDesktopLocator = "//p[contains(text()," +
                "\"Any updates you have made may not be shown until the request is approved by " +
                "the relevant organisation or healthcare provider.\")]",
        page = this,
        helpfulName = "Any updates you have made may not be shown until the request is approved by " +
                "the relevant organisation or healthcare provider. p"
    )

    fun assertHelpPageIsDisplayed(){
        missingOrIncorrectReferralsOrAppointmentsHelpPageMainTitle.assertIsVisible()
        missingOrIncorrectReferralsOrAppointmentsHelpPageTitle1.assertIsVisible()
        missingOrIncorrectReferralsOrAppointmentsHelpPageTitle2.assertIsVisible()
        missingOrIncorrectReferralsOrAppointmentsHelpPageText1.assertIsVisible()
        missingOrIncorrectReferralsOrAppointmentsHelpPageText2.assertIsVisible()
        missingOrIncorrectReferralsOrAppointmentsHelpPageText3.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}
