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

    private val missingAppointmentsHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If something is missing\")]",
        page = this,
        helpfulName = "If something is missing H2"
    )

    private val missingAppointmentsHelpText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have appointments not shown that " +
                "are in other services. Contact the healthcare provider " +
                "the appointment is booked with.\")]",
        page = this,
        helpfulName = "You may have appointments not shown that are in other services. Contact the " +
                "healthcare provider the appointment is booked with. p"
    )

    private val underEighteenHelpText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If you're aged 16 to 17, you may not be able to view or " +
                "manage some of your hospital appointments. This is because some NHS Trusts require you to " +
                "be aged 18 or over to access these appointments.\")]",
        page = this,
        helpfulName = "If you're aged 16 to 17, you may not be able to view or manage some " +
                "of your hospital appointments. This is because some NHS Trusts require " +
                "you to be aged 18 or over to access these appointments. p"
    )

    private val changeNotShowingHelpTitle = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"If a change or cancellation is not showing\")]",
        page = this,
        helpfulName = "If a change or cancellation is not showing H2"
    )

    private val changeNotShowingHelpTextOne = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"You may have requested to change or " +
                "permanently cancel a booked appointment. " +
                "This request may not automatically be accepted.\")]",
        page = this,
        helpfulName = "You may have requested to change or permanently cancel a booked appointment. "
                + "This request may not automatically be accepted. p"
    )

    private val changeNotShowingHelpTextTwo = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"The appointment will show as pending " +
                "while the request is reviewed by the relevant organisation or " +
                "healthcare provider it’s booked with.\")]",
        page = this,
        helpfulName = "The appointment will show as pending while the request is reviewed by " +
                "the relevant organisation or healthcare provider it’s booked with. p"
    )

    private val changeNotShowingHelpTextThree = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"If the request to change or cancel the appointment is " +
                "not accepted it will still show as booked.\")]",
        page = this,
        helpfulName = "If the request to change or cancel the appointment is " +
                "not accepted it will still show as booked. p"
    )

    fun assertHelpPageIsDisplayed(){
        confirmedAppointmentsHelpPageMainTitle.assertIsVisible()
        missingAppointmentsHelpTitle.assertIsVisible()
        missingAppointmentsHelpText.assertIsVisible()
        underEighteenHelpText.assertIsVisible()
        changeNotShowingHelpTitle.assertIsVisible()
        changeNotShowingHelpTextOne.assertIsVisible()
        changeNotShowingHelpTextTwo.assertIsVisible()
        changeNotShowingHelpTextThree.assertIsVisible()
        getBackLink().assertIsVisible()
    }
}



