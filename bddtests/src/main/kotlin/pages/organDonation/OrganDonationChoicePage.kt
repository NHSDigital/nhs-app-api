package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : HybridPageObject() {

    val organDonationTitle = HybridPageElement(
            webDesktopLocator = "//h2",
            webMobileLocator = "//h2",
            androidLocator = null,
            page = this,
            helpfulName = "Title"
    ).withText("Register your organ donation decision")


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
            page = this
    )
}