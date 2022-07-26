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

    private val somethingIsMissingTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val somethingIsMissingText1 = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have referrals being reviewed by a clinic " +
                "that are not shown but are in other services. Contact the organisation that referred you.\")]",
        page = this,
        helpfulName = "You may have referrals being reviewed by a clinic that are not " +
                "shown but are in other services. Contact the organisation that referred you. p"
    )

    private val cancellationNotShowingTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If a cancellation is not showing\")]",
        page = this,
        helpfulName = "If a cancellation is not showing H2"
    )

    private val cancellationNotShowingText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you have cancelled a referral that’s " +
                "being reviewed and it’s still showing you need to contact the " +
                "healthcare provider that referred you.\")]",
        page = this,
        helpfulName = "If you have cancelled a referral that’s being reviewed and it’s still " +
                "showing you need to contact the healthcare provider that referred you. p"
    )

    fun assertHelpPageIsDisplayed(){
        referralsInReviewHelpPageMainTitle.assertIsVisible()
        somethingIsMissingTitle.assertIsVisible()
        somethingIsMissingText1.assertIsVisible()
        cancellationNotShowingTitle.assertIsVisible()
        cancellationNotShowingText.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}
