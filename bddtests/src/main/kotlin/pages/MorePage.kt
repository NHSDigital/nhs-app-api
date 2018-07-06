package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/more")
open class MorePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val btnOrganDonation = HybridPageElement(
            browserLocator = "//a[contains(text(),'Set organ donation preferences')]",
            androidLocator = null,
            page = this
    )

    val donationHeading = HybridPageElement(
            browserLocator = "//h2[contains(text(),'Organ donation preferences')]",
            androidLocator = null,
            page = this
    )

    val donationDescription = HybridPageElement(
            browserLocator = "//p[contains(text(),'Help us save thousands of lives in the UK every year by signing up to become an organ donor. Register your decision and choose what you want to donate on the NHS Organ Donor Register.')]",
            androidLocator = null,
            page = this
    )

    fun clickOrganDonations() {
        findByXpath("//*[@id='btn_organdonation']").click()
    }

    fun clickDataSharing() {
        findByXpath("//*[@id='btn_datasharing']").click()
    }

    fun isDonationHeaderVisible(): Boolean {
        return donationHeading.element.isCurrentlyEnabled
    }

    fun isDonationDescriptionVisible(): Boolean {
        return donationDescription.element.isCurrentlyEnabled
    }

    fun isDonationButtonVisible(): Boolean {
        return btnOrganDonation.element.isCurrentlyEnabled
    }
}
