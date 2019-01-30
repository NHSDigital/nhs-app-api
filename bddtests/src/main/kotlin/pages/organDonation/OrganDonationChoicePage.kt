package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Register your organ donation decision"

    val noButton = HybridPageElement(
            webDesktopLocator = "//button[descendant::*[contains(text(),\"NO\")]]",
            webMobileLocator = "//button[descendant::*[contains(text(),\"NO\")]]",
            androidLocator = null,
            page = this
    )

    val yesButton = HybridPageElement(
            webDesktopLocator = "//button[descendant::*[contains(text(),\"YES\")]]",
            webMobileLocator = "//button[descendant::*[contains(text(),\"YES\")]]",
            androidLocator = null,
            page = this,
            helpfulName = "No option"
    )

    override fun assertDisplayed() {
        title.assertIsVisible()
        noButton.assertIsVisible()
        yesButton.assertIsVisible()
    }
}