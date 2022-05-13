package pages.wayfinder.help

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder/help/referrals-in-review-help")
open class ReferralsInReviewHelpPage : HybridPageObject() {
    private val referralsInReviewHelpPageMainTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Missing or incorrect referrals in review\")]",
        page = this,
        helpfulName = "Missing or incorrect referrals in review H1"
    )

    private val referralsInReviewHelpPageTitle1 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val referralsInReviewHelpPageTitle2 = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If a cancellation is not showing\")]",
        page = this,
        helpfulName = "If a cancellation is not showing H2"
    )

    private val referralsInReviewHelpPageText1 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have referrals being reviewed by a clinic " +
                "that are not shown but are in other services. Contact the organisation that referred you.\")]",
        page = this,
        helpfulName = "You may have referrals being reviewed by a clinic that are not " +
                "shown but are in other services. Contact the organisation that referred you. p"
    )

    private val referralsInReviewHelpPageText2 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have requested to cancel a referral " +
                "that's being reviewed.\")]",
        page = this,
        helpfulName = "You may have requested to cancel a referral that's being reviewed. p"
    )

    private val referralsInReviewHelpPageText3 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"Your cancellation may not be shown until " +
                "the request is approved by the organisation that referred you.\")]",
        page = this,
        helpfulName = "Your cancellation may not be shown until the request is approved by " +
                "the organisation that referred you. p"
    )

    fun assertHelpPageIsDisplayed(){
        referralsInReviewHelpPageMainTitle.assertIsVisible()
        referralsInReviewHelpPageTitle1.assertIsVisible()
        referralsInReviewHelpPageTitle2.assertIsVisible()
        referralsInReviewHelpPageText1.assertIsVisible()
        referralsInReviewHelpPageText2.assertIsVisible()
        referralsInReviewHelpPageText3.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}
