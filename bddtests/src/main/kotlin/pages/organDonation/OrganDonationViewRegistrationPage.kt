package pages.organDonation

import mocking.organDonation.models.FaithDeclaration
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.sharedElements.BannerObject

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationViewRegistrationPage : OrganDonationBasePage() {

    override fun assertDisplayed() {
        waitForSpinnerToDisappear()
    }

    override val titleText: String = "Your decision"

    val decisionModule by lazy { OrganDonationYourDecisionModule(this) }

    val amendDecisionLink = getLink("I want to change my decision")

    val reaffirmDecisionLink = getLink("This is still my decision")

    val nextSteps = OrganDonationNextStepsModule(this)

    val otherThings = OrganDonationOtherThingsModule(this)

    fun assertCreatedBanner() {
        BannerObject.success(this).assertVisible("Your decision has been recorded")
        title.assertIsVisible()
        amendDecisionLink.assertIsVisible()
    }

    fun assertDecisionSubmitted() {
        BannerObject.success(this, "Decision submitted")
                .assertVisible("We have successfully received your organ donation decision.")

        val bodyTextDecisionFound = arrayOf(
                "We will process your registration and you will then be able to view and amend this via the NHS App. " +
                        "This may take up to 2 working days.")
        OrganDonationDetailsAssertor.withH2Header("What happens next", this)
                .assert(bodyTextDecisionFound)
    }

    fun assertDecisionFound() {
        waitForSpinnerToDisappear()

        BannerObject.success(this, "Decision found")
                .assertVisible("Your registration is currently being processed.")

        val bodyTextDecisionFound = arrayOf(
                "Please check back in 2 working days. " +
                        "You’ll then be able to view and amend your decision via the NHS App.")
        OrganDonationDetailsAssertor.withH2Header("We are still processing your registration", this)
                .assert(bodyTextDecisionFound)
    }

    private val faithYesText = "When I die, I would like NHS staff to speak with my family " +
            "(and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs."
    private val faithNoText = "When I die, I do not want NHS staff to speak with my family " +
            "(and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs."
    private val faithPreferNotToSayText = "I prefer not to say whether I want NHS staff to speak with my family " +
            "(and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs."

    fun assertFaithTextIsNotPresent() {
        faithSection.assertElementNotPresent()
    }

    fun assertFaithTextIsPresent(faith: FaithDeclaration) {
        val map = mapOf(FaithDeclaration.Yes to faithYesText,
                FaithDeclaration.No to faithNoText,
                FaithDeclaration.NotStated to faithPreferNotToSayText)

        Assert.assertTrue(
                "Test setup incorrect, map does not contain value for $faith",
                map.containsKey(faith))

        Assert.assertEquals("Expected Faith text", map[faith], faithSection.element.text.trim())
    }

    private val faithSection = HybridPageElement(
            "//div[@id='faithAndBeliefs']",
            page = this,
            helpfulName = "Faith and Beliefs section")
}
