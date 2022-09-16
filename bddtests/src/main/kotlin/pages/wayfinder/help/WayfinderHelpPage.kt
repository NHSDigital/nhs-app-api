package pages.wayfinder.help

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/hospital-referrals-appointments/help")
open class WayfinderHelpPage : HybridPageObject() {
    private val referralsHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"Referrals\")]",
        page = this,
        helpfulName = "Referrals H2"
    )

    private val missingReferralsExpanderTitle = HybridPageElement(
        webDesktopLocator = "//div[contains(text(),\"Missing referrals\")]",
        page = this,
        helpfulName = "Missing referrals div"
    )

    private val missingReferralsText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have referrals not shown that " +
                                                  "are in other services. Contact the " +
                                                  "healthcare provider that referred you " +
                                                  "for more information.\")]",
        page = this,
        helpfulName = "Missing referrals text p"
    )

    private val incorrectOrCancelledReferralsExpanderTitle = HybridPageElement(
        webDesktopLocator = "//div[contains(text(),\"Incorrect or cancelled referrals\")]",
        page = this,
        helpfulName = "Incorrect or cancelled referrals div"
    )

    private val cancelledReferralsContactText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you have cancelled a referral " +
                                                  "and it's still showing, you need to contact " +
                                                  "the healthcare provider that referred you.\")]",
        page = this,
        helpfulName = "Cancelled referrals contact text p"
    )

    private val appointmentsHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"Appointments\")]",
        page = this,
        helpfulName = "Appointments H2"
    )

    private val missingAppointmentsExpanderTitle = HybridPageElement(
        webDesktopLocator = "//div[contains(text(),\"Missing appointments\")]",
        page = this,
        helpfulName = "Missing appointments div"
    )

    private val missingAppointmentsTextOne = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have appointments not shown that are " +
                                                  "in other services. Contact the relevant organisation " +
                                                  "or healthcare provider for more information.\")]",
        page = this,
        helpfulName = "Missing appointments text one p"
    )

    private val missingAppointmentsTextTwo = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you're aged 16 to 17, you may not be able to " +
                                                  "view or manage some of your hospital appointments. " +
                                                  "This is because some NHS Trusts require you to be " +
                                                  "aged 18 or over to access these appointments.\")]",
        page = this,
        helpfulName = "Missing appointments text two p"
    )

    private val incorrectChangedCancelledAppointmentsExpanderTitle = HybridPageElement(
        webDesktopLocator = "//div[contains(text(),\"Incorrect, changed or cancelled appointments\")]",
        page = this,
        helpfulName = "Incorrect changed or cancelled appointments div"
    )

    private val incorrectChangedCancelledAppointmentsTextOne = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have requested to change or permanently " +
                                                  "cancel a confirmed appointment. " +
                                                  "This request may not automatically be accepted.\")]",
        page = this,
        helpfulName = " p"
    )

    private val incorrectChangedCancelledAppointmentsTextTwo = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"The appointment will show as pending while the request " +
                                                  "is reviewed by the relevant organisation or healthcare " +
                                                  "provider it's booked with.\")]",
        page = this,
        helpfulName = " p"
    )

    private val incorrectChangedCancelledAppointmentsTextThree = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If the request to change or cancel the appointment is not " +
                                                  "accepted it will still show as booked in your " +
                                                  "confirmed appointments.\")]",
        page = this,
        helpfulName = " p"
    )

    private val incorrectChangedCancelledAppointmentsTextFour = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If the cancellation is accepted the appointment will " +
                                                  "show as cancelled in your confirmed appointments.\")]",
        page = this,
        helpfulName = " p"
    )

    fun assertHelpPageIsDisplayed(){
        referralsHelpTitle.assertIsVisible()
        missingReferralsExpanderTitle.assertIsVisible()
        missingReferralsText.assertIsVisible()
        incorrectOrCancelledReferralsExpanderTitle.assertIsVisible()
        cancelledReferralsContactText.assertIsVisible()
        appointmentsHelpTitle.assertIsVisible()
        missingAppointmentsExpanderTitle.assertIsVisible()
        missingAppointmentsTextOne.assertIsVisible()
        missingAppointmentsTextTwo.assertIsVisible()
        incorrectChangedCancelledAppointmentsExpanderTitle.assertIsVisible()
        incorrectChangedCancelledAppointmentsTextOne.assertIsVisible()
        incorrectChangedCancelledAppointmentsTextTwo.assertIsVisible()
        incorrectChangedCancelledAppointmentsTextThree.assertIsVisible()
        incorrectChangedCancelledAppointmentsTextFour.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}
