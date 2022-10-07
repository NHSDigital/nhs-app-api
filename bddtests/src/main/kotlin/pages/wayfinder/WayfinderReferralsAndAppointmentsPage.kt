package pages.wayfinder

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/hospital-referrals-appointments")
open class WayfinderReferralsAndAppointmentsPage : HybridPageObject() {

    private val noSpecialtyInReview = "Your healthcare provider has requested for you to be referred. " +
                                      "This request is being reviewed. You do not need to do anything."
    private val noSpecialtyRebook = "You need to rebook your referral appointment " +
                                    "as the one you had booked has been cancelled."

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Referrals, hospital and other appointments\")]",
        page = this,
        helpfulName = "Wayfinder Referrals And Appointments - h1"
    )

    private val noConfirmedAppointmentsToViewOrManageText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You have no confirmed appointments to view or manage.\")]",
        page = this,
        helpfulName = "No confirmed appointments to manage = paragraph"
    )

    private val readyToConfirmAppointmentHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to confirm appointment\")]",
        page = this,
        helpfulName = "Ready to confirm appointment - h3"
    )

    private val referralInReviewHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Referral request in review\")]",
        page = this,
        helpfulName = "Referral in review - h3"
    )

    private val referralReadyToRebookHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to rebook\")]",
        page = this,
        helpfulName = "Referral ready to rebook - h3"
    )

    private val referralInReviewOverdueHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Review by clinic overdue\")]",
        page = this,
        helpfulName = "Referral in review overdue - h3"
    )

    private val referralReadyToBookHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Ready to book\")]",
        page = this,
        helpfulName = "Referral ready to book - h3"
    )

    private val appointmentBookedHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Booked appointment\")]",
        page = this,
        helpfulName = "Appointment booked - h3"
    )

    private val appointmentCancelledHeading = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Cancelled appointment\")]",
        page = this,
        helpfulName = "Appointment cancelled - h3"
    )

    private val referralInReviewWithNoSpecialty = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"${noSpecialtyInReview}\")]",
        page = this,
        helpfulName = "Referral in review description with no specialty - paragraph"
    )

    private val referralReadyToRebookWithNoSpecialty = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"${noSpecialtyRebook}\")]",
        page = this,
        helpfulName = "Referral ready to rebook description with no specialty - paragraph"
    )

    private val referralsOrAppointmentsHelpLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"What to do if a referral or appointment is missing, " +
                                                   "incorrect or has not been cancelled\")]",
        page = this,
        helpfulName = "Missing referral or appointment, incorrect or has not been cancelled - h2"
    )

    private val appointmentsHelpLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"What to do if an appointment is missing, " +
                                                   "incorrect or has not been changed or cancelled\")]",
        page = this,
        helpfulName = "Missing appointment, incorrect or has not been cancelled - h2"
    )

    private val referralsInReviewHelpLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"What to do if a referral being reviewed by " +
                                                   "a clinic is missing or incorrect\")]",
        page = this,
        helpfulName = "Missing referral in review or incorrect - h2"
    )

    private val waitTimesJumpLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"Waiting lists\")]",
        page = this,
        helpfulName = "Wait times - h2"
    )

    private val referralsOrAppointmentsHelpTitle = "What to do if a referral or appointment " +
                                                   "is missing, incorrect or has not been cancelled"
    private val confirmedAppointmentsHelpTitle = "What to do if an appointment is missing, " +
                                                 "incorrect or has not been changed or cancelled"
    private val referralsInReviewHelpTitle = "What to do if a referral being reviewed by " +
                                             "a clinic is missing or incorrect"
    private val waitTimesTitle = "Waiting lists"

    var content = LinksWithDescriptionsContent(
        linkBlockTitle = "Wayfinder Help links",
        containerXPath = "//div[@class='nhsuk-grid-column-full']",
        linkStyling = "h2")
        .addLink(referralsOrAppointmentsHelpTitle, "")
        .addLink(confirmedAppointmentsHelpTitle, "")
        .addLink(referralsInReviewHelpTitle, "")
        .addLink(waitTimesTitle, "")

    val links by lazy { LinksElement(this, content) }

    val missingOrIncorrectReferralsOrAppointmentsLink by lazy {
        links.link("What to do if a referral or appointment is missing, incorrect or has not been cancelled")
    }

    val appointmentsLink by lazy {
        links.link("What to do if an appointment is missing, incorrect or has not been changed or cancelled")
    }

    val referralsInReviewLink by lazy {
        links.link("What to do if a referral being reviewed by a clinic is missing or incorrect")
    }

    val waitTimesLink by lazy {
        links.link("Waiting lists")
    }

    fun assertWayfinderTitleIsDisplayed() {
        pageTitle.assertIsVisible()
    }
    
    fun assertNoConfirmedAppointmentsMessageIsDisplayed() {
        noConfirmedAppointmentsToViewOrManageText.assertIsVisible()
    }

    fun assertAppointmentReadyToConfirmIsDisplayed() {
        readyToConfirmAppointmentHeading.assertIsVisible()
    }

    fun assertBookedAppointmentIsDisplayed() {
        appointmentBookedHeading.assertIsVisible()
    }

    fun assertAppointmentCancelledIsDisplayed(){
        appointmentCancelledHeading.assertIsVisible()
    }

    fun assertReferralInReviewIsDisplayed(){
        referralInReviewHeading.assertIsVisible()
    }

    fun assertReferralReadyToRebookIsDisplayed() {
        referralReadyToRebookHeading.assertIsVisible()
    }

    fun assertReferralInReviewOverdueIsDisplayed() {
        referralInReviewOverdueHeading.assertIsVisible()
    }

    fun assertReferralReadyToBookIsDisplayed() {
        referralReadyToBookHeading.assertIsVisible()
    }

    fun assertReferralInReviewWithNoSpecialtyIsDisplayed() {
        referralInReviewWithNoSpecialty.assertIsVisible()
    }

    fun assertReferralReadyToRebookWithNoSpecialtyIsDisplayed() {
        referralReadyToRebookWithNoSpecialty.assertIsVisible()
    }

    fun assertReferralsOrAppointmentsHelpLinkIsDisplayed(){
        referralsOrAppointmentsHelpLink.assertIsVisible()
    }

    fun assertAppointmentsHelpLinkIsDisplayed(){
        appointmentsHelpLink.assertIsVisible()
    }

    fun assertReferralsInReviewHelpLinkIsDisplayed(){
        referralsInReviewHelpLink.assertIsVisible()
    }

    fun assertWaitTimesLinkIsDisplayed(){
        waitTimesJumpLink.assertIsVisible()
    }
}
