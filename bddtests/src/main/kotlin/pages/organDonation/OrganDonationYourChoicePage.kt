package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationYourChoicePage : HybridPageObject() {

    val yourChoiceTitle = HybridPageElement(
            "//h2",
            null,
            null,
            this,
            "header").withText("Your choice")

    val radioButtons by lazy {
        RadioButtons(HybridPageElement(
                "//label[input]",
                null,
                null,
                this,
                "Radio Buttons"))
    }

    private val expectedOptions by lazy {
        arrayListOf(
                Pair(allOfMyOrgans, "Help up to nine people through organ donation and even more through tissue")
        )
    }

    val allOfMyOrgans: String = "All my organs and tissue"

    fun assertOptions() {
        radioButtons.assertAreEqual(expectedOptions)
    }
}
