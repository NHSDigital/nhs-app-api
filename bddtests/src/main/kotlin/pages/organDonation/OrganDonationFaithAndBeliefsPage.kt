package pages.organDonation

import mocking.organDonation.models.FaithDeclaration
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertSingleElementPresent
import pages.sharedElements.ExpandElement
import pages.sharedElements.RadioButtons
import pages.sharedElements.TextBlockElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationFaithAndBeliefsPage : OrganDonationBasePage() {

    override val titleText: String = "Faith / beliefs"

    val radioButtons by lazy { RadioButtons.create(this) }

    val noOption = "No - this is not applicable to me"
    val yesOption = "Yes - this is applicable to me"
    val preferNotToSayOption = "Prefer not to say"

    private val expectedOptions by lazy {
        arrayListOf(yesOption,
                noOption,
                preferNotToSayOption)
    }

    val endOfLifeWishes = ExpandElement(this)

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertInformation()
        radioButtons.assertAreEqual(expectedOptions)
    }

    fun selectOption(faith:FaithDeclaration) {
        radioButtons.button(OrganDonationFaithModule.getFaith(faith)).select()
    }

    private fun assertInformation() {
        TextBlockElement.withH2Header("Faith / beliefs", this)
                .assert("When you die, NHS staff can ask your family (and anyone else appropriate) about your " +
                         "faith and beliefs. This is how NHS staff will find out about any end of life wishes you " +
                        "might have.")
                .assert("Record here whether you want our specialist nurses to discuss your faith or " +
                        "beliefs with your family when you die, at the same time they approach them about " +
                        "organ donation.")
        question.assertSingleElementPresent()
    }

    private var questionContent =
            "I would like NHS staff to speak to my family and anyone else appropriate about how organ donation " +
                    "can go ahead in line with my faith or beliefs."

    private val question = HybridPageElement(
            "//b[normalize-space() = \"$questionContent\"]",
            page = this,
            helpfulName = "question")
}