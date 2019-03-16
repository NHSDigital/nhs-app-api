package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertSingleElementPresent
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationFaithAndBeliefsPage : OrganDonationBasePage() {

    override val titleText: String = "Faith / beliefs"

    val radioButtons by lazy { RadioButtons.create(this)}

    private val expectedOptions by lazy {
        arrayListOf(
                Pair("Yes", "I want NHS staff to talk to my family (and other relevant people) " +
                        "about how organ donation works with my faith/beliefs."),
                Pair("No", "I do not want NHS staff to talk to anyone about organ " +
                        "donation and my faith/beliefs."),
                Pair("Prefer not to say", "")
        )
    }

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertInformation()
        radioButtons.assertAreEqual(expectedOptions)
    }

    private fun assertInformation() {
        val bodyTextOne = arrayOf(
                "When organ donation is a possibility, NHS staff will always speak to a donor's family " +
                "about the donor's decision, medical history, and anything else that would be relevant to " +
                "organ donation.",
                "We recognise that for some people, this will include their faith or beliefs and they would " +
                        "want organ donation to go ahead in a way that is in line with their beliefs or customs.")
        val bodyTextTwo = arrayOf("Let us know if you want your faith and beliefs to be a part of " +
                        "discussions between NHS staff, your family, and anyone suggested by " +
                        "your family, when organ donation is a possibility.")
        OrganDonationDetailsAssertor.withH3Header("Why they matter for organ donation", this)
                .assert(bodyTextOne)
        OrganDonationDetailsAssertor.withH3Header("What we can do to help", this)
                .assert(bodyTextTwo)
        question.assertSingleElementPresent()
    }

    private var questionContent =
            "Would you like NHS staff to speak to your family (and anyone else appropriate) " +
            "about how organ donation can go ahead in line with your faith or beliefs?"

    private val question = HybridPageElement(
            "//b[normalize-space() = \"$questionContent\"]",
            page = this,
            helpfulName = "question")
}