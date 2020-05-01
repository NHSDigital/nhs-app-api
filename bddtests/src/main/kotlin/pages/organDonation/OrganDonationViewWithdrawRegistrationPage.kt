package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.assertIsVisible
import pages.avoidChromeWebDriverServiceCrash
import pages.sharedElements.BannerObject
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationViewWithdrawRegistrationPage: OrganDonationBasePage()  {
    override fun assertDisplayed() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        title.assertIsVisible()
    }
    override val titleText = "What to do next"

    val otherThings = OrganDonationOtherThingsModule(this)
    val nextSteps = OrganDonationNextStepsModule(this)

    fun assertDecisionWithdrawn() {
        val bannerText = arrayListOf("You no longer have a decision recorded on the NHS Organ Donor Register.",
                "If you die in circumstances where donation is possible, " +
                        "it will be considered that you have agreed to be an organ donor " +
                        "unless you are in an excluded group.",
                "More information about these changes to the law around organ donation",
                "You can record a new decision at any time.")
        BannerObject.success(this, "Decision withdrawn").assertVisible(bannerText)

        val expected = ExpectedPageStructure().h2("What to do next")
                .paragraph("Let your family know that you have withdrawn your decision from the register. " +
                        "They will not know what you want unless you tell them.")

        expected.assert(this)
    }
}
