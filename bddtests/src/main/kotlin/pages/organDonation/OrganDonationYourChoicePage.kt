package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationYourChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Your choice"

    val radioButtons by lazy {
        RadioButtons.create(this)
    }

    private val expectedOptions by lazy {
        arrayListOf(
                Pair(allOfMyOrgans,
                        "Help up to nine people through organ donation and even more through tissue donation."),
                Pair(someOfMyOrgans, "Choose which of your organs and tissue to donate.")
        )
    }

    val allOfMyOrgans: String = "All my organs and tissue"
    val someOfMyOrgans: String = "Some organs and tissue"

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertOptions()
    }

    fun assertOptions() {
        radioButtons.assertAreEqual(expectedOptions)
    }
}