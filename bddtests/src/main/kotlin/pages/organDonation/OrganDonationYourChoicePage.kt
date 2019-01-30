package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationYourChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Your choice"

    val radioButtons by lazy {
        RadioButtons(HybridPageElement(
                "//div[input][label]",
                "//div[input][label]",
                null,
                null,
                this,
                "Radio Buttons"))
    }

    private val expectedOptions by lazy {
        arrayListOf(
                Pair(allOfMyOrgans, "Help up to nine people through organ donation and even more through tissue"),
                Pair(someOfMyOrgans, "Choose which of your organs and tissue to donate")
        )
    }

    val allOfMyOrgans: String = "All my organs and tissue"
    val someOfMyOrgans: String = "Specific organs and tissue"

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertOptions()
    }

    fun assertOptions() {
        radioButtons.assertAreEqual(expectedOptions)
    }
}