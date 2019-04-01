package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.assertIsVisible
import pages.sharedElements.BannerObject
import pages.sharedElements.TextBlockElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationViewWithdrawRegistrationPage: OrganDonationBasePage()  {
    override fun assertDisplayed() {
        title.assertIsVisible()
    }
    override val titleText = "What to do next"

    val otherThings = OrganDonationOtherThingsModule(this)
    val nextSteps = OrganDonationNextStepsModule(this)

    fun assertDecisionWithdrawn() {
        val bannerText = arrayListOf("You no longer have a decision recorded on the NHS Organ Donor Register.",
                "You can record a new decision at any time.")
        BannerObject.success(this, "Decision withdrawn").assertVisible(bannerText)

        TextBlockElement.withH2Header("What to do next", this)
                .assert("Let your family know that you have withdrawn your details from the register. " +
                        "If you die in circumstances where donation is possible, " +
                        "we will ask your family if you expressed a verbal decision. " +
                        "If you did not express a verbal decision, " +
                        "we will ask your family to make a decision on your behalf.",
                        "Your family won't know what you want unless you tell them, " +
                                "so help them now to support your decision at a difficult time.")
    }
}