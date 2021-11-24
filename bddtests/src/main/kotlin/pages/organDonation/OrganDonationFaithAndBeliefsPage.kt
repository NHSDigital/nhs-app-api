package pages.organDonation

import mocking.organDonation.models.FaithDeclaration
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.ExpandElement
import pages.sharedElements.RadioButtons

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
        radioButtons.assertAreEqual(expectedOptions)
    }

    fun selectOption(faith:FaithDeclaration) {
        radioButtons.button(OrganDonationFaithModule.getFaith(faith)).select()
    }
}
