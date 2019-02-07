package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Register your organ donation decision"

    val noButton = button("NO")

    val yesButton = button("YES")

    private fun button(option: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//button[descendant::*[contains(text(),\"$option\")]]",
                webMobileLocator = "//button[descendant::*[contains(text(),\"$option\")]]",
                androidLocator = null,
                page = this,
                helpfulName = "$option option"
        )
    }

    override fun assertDisplayed() {
        title.assertIsVisible()
        noButton.assertIsVisible()
        yesButton.assertIsVisible()
    }
}