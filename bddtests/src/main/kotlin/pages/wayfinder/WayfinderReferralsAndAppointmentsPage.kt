package pages.wayfinder

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/wayfinder-appointments")
open class WayfinderReferralsAndAppointmentsPage : HybridPageObject() {

    private val pageTitleWithData = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Referrals, hospital and other appointments\")]",
        page = this,
        helpfulName = "Wayfinder Referrals And Appointments Title"
    )

    private val pageTitleWithNoData = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"No referrals or appointments to view and manage\")]",
        page = this,
        helpfulName = "Wayfinder No Referrals And Appointments Title"
    )

    private val inReviewReferralHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Your referral request is being reviewed\")]",
        page = this,
        helpfulName = "In Review Referral H3"
    )

    private val bookableCancelledReferralHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to rebook\")]",
        page = this,
        helpfulName = "Bookable Cancelled Referral H3"
    )

    fun assertWayfinderWithDataTitleIsDisplayed() {
        pageTitleWithData.assertIsVisible()
    }

    fun assertWayfinderWithoutDataTitleIsDisplayed() {
        pageTitleWithNoData.assertIsVisible()
    }

    fun assertInReviewReferralDisplayed(){
        inReviewReferralHeading.assertIsVisible()
    }

    fun assertBookableCancelledReferralDisplayed() {
        bookableCancelledReferralHeading.assertIsVisible()
    }
}
