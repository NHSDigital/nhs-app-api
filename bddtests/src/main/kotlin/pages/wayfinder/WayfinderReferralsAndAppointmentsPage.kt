package pages.wayfinder

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder")
open class WayfinderReferralsAndAppointmentsPage : HybridPageObject() {

    private val noSpecialityInReview = "Your healthcare provider has requested for you to be referred. " +
            "This request is being reviewed. You do not need to do anything."
    private val noSpecialityRebook = "You need to rebook your referral appointment " +
            "as the one you had booked has been cancelled."

    private val pageTitleWithData = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Referrals, hospital and other appointments\")]",
        page = this,
        helpfulName = "Wayfinder Referrals And Appointments Title"
    )

    private val pageTitleWithNoData = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"You have no referrals or appointments to view or manage\")]",
        page = this,
        helpfulName = "Wayfinder No Referrals And Appointments Title"
    )

    private val appointmentToConfirmHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to confirm appointment\")]",
        page = this,
        helpfulName = "Appointment to confirm H3"
    )

    private val noUpcomingAppointments = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You have no confirmed appointments to view or manage.\")]",
        page = this,
        helpfulName = "Appointment to confirm H3"
    )

    private val inReviewReferralHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Referral request in review\")]",
        page = this,
        helpfulName = "In Review Referral H3"
    )

    private val bookableCancelledReferralHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to rebook\")]",
        page = this,
        helpfulName = "Bookable Cancelled Referral H3"
    )

    private val bookableOverdueReferralHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Review by clinic is overdue\")]",
        page = this,
        helpfulName = "Bookable Overdue H3"
    )

    private val bookableAwaitingBookHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to book\")]",
        page = this,
        helpfulName = "Bookable Awaiting Booking H3"
    )

    private val appointmentBookedHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Booked appointment\")]",
        page = this,
        helpfulName = "Appointment booked H3"
    )

    private val noSpecialityReferencedInReview = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"${noSpecialityInReview}\")]",
        page = this,
        helpfulName = "No speciality referenced in review paragraph"
    )

    private val noSpecialityReferencedRebook = HybridPageElement(
            webDesktopLocator = "//p[contains(text(),\"${noSpecialityRebook}\")]",
            page = this,
            helpfulName = "No speciality referenced in review paragraph"
    )

    fun assertWayfinderWithDataTitleIsDisplayed() {
        pageTitleWithData.assertIsVisible()
    }

    fun assertWayfinderWithoutDataTitleIsDisplayed() {
        pageTitleWithNoData.assertIsVisible()
    }

    fun assertNoUpcomingAppointmentsDisplayed() {
        noUpcomingAppointments.assertIsVisible()
    }

    fun assertApointmentToConfirmIsDisplayed() {
        appointmentToConfirmHeading.assertIsVisible()
    }

    fun assertBookedAppointmentIsDisplayed() {
        appointmentBookedHeading.assertIsVisible()
    }

    fun assertInReviewReferralDisplayed(){
        inReviewReferralHeading.assertIsVisible()
    }

    fun assertBookableCancelledReferralDisplayed() {
        bookableCancelledReferralHeading.assertIsVisible()
    }

    fun assertBookableOverdueReferralDisplayed() {
        bookableOverdueReferralHeading.assertIsVisible()
    }

    fun assertBookableAwaitingBookDisplayed() {
        bookableAwaitingBookHeading.assertIsVisible()
    }

    fun assertNoSpecialityReferencedForInReview() {
        noSpecialityReferencedInReview.assertIsVisible()
    }

    fun assertNoSpecialityReferencedForRebook() {
        noSpecialityReferencedRebook.assertIsVisible()
    }
}
