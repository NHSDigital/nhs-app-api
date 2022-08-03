package pages.wayfinder.help

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder/help/referrals-or-appointments-help")
open class ReferralsOrAppointmentsHelpPage : HybridPageObject() {
    private val missingOrIncorrectReferralsOrAppointmentsHelpPageMainTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Missing, incorrect or cancelled referrals or appointments\")]",
        page = this,
        helpfulName = "Missing, incorrect or cancelled referrals or appointments H1"
    )

    private val missingAppointmentsHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val missingAppointmentsHelpText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have referrals or appointments not shown " +
                "that are in other services. Contact the relevant organisation or healthcare provider.\")]",
        page = this,
        helpfulName = "You may have referrals or appointments not shown that are in other services. " +
                "Contact the relevant organisation or healthcare provider. p"
    )

    private val underEighteenHelpText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you're aged 16 to 17, you may not be able to view or " +
                "manage some of your hospital appointments. This is because some NHS Trusts require you to " +
                "be aged 18 or over to access these appointments.\")]",
        page = this,
        helpfulName = "If you're aged 16 to 17, you may not be able to view or manage some of your " +
                "hospital appointments. This is because some NHS Trusts require you to be aged 18 " +
                "or over to access these appointments. p"
    )

    private val changeNotShowingHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"Cancelled referrals\")]",
        page = this,
        helpfulName = "Cancelled referrals H2"
    )

    private val changeNotShowingHelpText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you have cancelled a referral and it’s still showing, " +
                "you need to contact the healthcare provider that referred you.\")]",
        page = this,
        helpfulName = "If you have cancelled a referral and it’s still showing, you need to " +
                "contact the healthcare provider that referred you. p"
    )

    private val appointmentChangeOrCancellationNotShowingTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If an appointment change or cancellation is not showing\")]",
        page = this,
        helpfulName = "If an appointment change or cancellation is not showing h2"
    )

    private val changeOrCancellationTextOne = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you have requested to change or permanently cancel " +
                "an appointment this request may not automatically be accepted.\")]",
        page = this,
        helpfulName = "If you have requested to change or permanently cancel " +
                "an appointment this request may not automatically be accepted. p"
    )

    private val changeOrCancellationTextTwo = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"The appointment will show as pending while the " +
                "request is reviewed by the relevant organisation or healthcare provider " +
                "it’s booked with.\")]",
        page = this,
        helpfulName = "The appointment will show as pending while the request is reviewed by " +
                "the relevant organisation or healthcare provider it’s booked with. p"
    )

    private val changeOrCancellationTextThree = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If the request to change or cancel " +
                "the appointment is not accepted it will still show as booked.\")]",
        page = this,
        helpfulName = "If the request to change or cancel the appointment is " +
                "not accepted it will still show as booked. p"
    )

    fun assertHelpPageIsDisplayed(){
        missingOrIncorrectReferralsOrAppointmentsHelpPageMainTitle.assertIsVisible()
        missingAppointmentsHelpTitle.assertIsVisible()
        missingAppointmentsHelpText.assertIsVisible()
        underEighteenHelpText.assertIsVisible()
        changeNotShowingHelpTitle.assertIsVisible()
        changeNotShowingHelpText.assertIsVisible()
        appointmentChangeOrCancellationNotShowingTitle.assertIsVisible()
        changeOrCancellationTextOne.assertIsVisible()
        changeOrCancellationTextTwo.assertIsVisible()
        changeOrCancellationTextThree.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}
