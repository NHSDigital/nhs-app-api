package pages.organDonation

import mocking.organDonation.models.FaithDeclaration
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.avoidChromeWebDriverServiceCrash
import pages.sharedElements.BannerObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.sharedElements.expectedPage.ParsedPage
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationViewRegistrationPage : OrganDonationBasePage() {

    override fun assertDisplayed() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        title.assertIsVisible()
    }

    override val titleText: String = "Your decision"

    val decisionModule by lazy { OrganDonationYourDecisionModule(this) }

    val amendDecisionLink = getLink("I want to change my decision")

    val reaffirmDecisionLink = getLink("This is still my decision")

    val nextSteps = OrganDonationNextStepsModule(this)

    val otherThings = OrganDonationOtherThingsModule(this)

    fun assertCreatedBanner() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        BannerObject.success(this).assertVisible("Your decision has been recorded")
        title.assertIsVisible()
        amendDecisionLink.assertIsVisible()
    }

    fun assertDecisionSubmitted() {
        val expected = ExpectedPageStructure()
                .h2("Decision submitted")
                .paragraph("We have successfully received your organ donation decision.")
                .h2("What happens next")
                .paragraph("We will process your registration and you will then be able to view and amend this " +
                        "via the NHS App. This may take up to 2 working days.")
        val actual = ParsedPage.parse(this,
                "//div[h2[normalize-space()='What happens next']]")
        ExpectedPageStructureAssertor().assert(actual, expected.build())
    }

    fun assertDecisionFound() {
        BannerObject.success(this, "Decision found")
                .assertVisible("Your registration is currently being processed.")

        val expected = ExpectedPageStructure()
                .h2("Decision found")
                .paragraph("Your registration is currently being processed.")
                .h2("We are still processing your registration")
                .paragraph("Please check back in 2 working days. " +
                                "You’ll then be able to view and amend your decision via the NHS App.")

        val actual = ParsedPage.parse(this,
                "//div[h2[normalize-space()='We are still processing your registration']]")

        ExpectedPageStructureAssertor().assert(actual, expected.build())
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

        Assert.assertEquals("Expected Faith text", map[faith], faithSection.text.trim())
    }

    private val faithSection = HybridPageElement(
            "//p[@id='faithAndBeliefs']",
            page = this,
            helpfulName = "Faith and Beliefs section")
}
