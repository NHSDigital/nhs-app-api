package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/more")
open class MorePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val btnOrganDonation = HybridPageElement(
            browserLocator = "//a/*[contains(text(),'Set organ donation preferences')]",
            androidLocator = null,
            page = this
    )

    val donationDescription = HybridPageElement(
            browserLocator = "//p[contains(text(),'Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register.')]",
            androidLocator = null,
            page = this
    )

    val btnDataSharing = HybridPageElement(
            browserLocator = "//a/*[contains(text(),'Manage your choice for sharing data')]",
            androidLocator = null,
            page = this
    )

    val dataSharingDescription = HybridPageElement(
            browserLocator = "//p[contains(text(),'Find out why your data matters and choose whether or not it can be used for research and planning.')]",
            androidLocator = null,
            page = this
    )

    fun isDonationButtonVisible(): Boolean {
        return btnOrganDonation.element.isCurrentlyVisible
    }

    fun isDonationDescriptionVisible(): Boolean {
        return donationDescription.element.isCurrentlyVisible
    }

    fun clickOrganDonations() {
        btnOrganDonation.element.click()
    }

    fun isDataSharingButtonVisible(): Boolean {
        return btnDataSharing.element.isCurrentlyVisible
    }

    fun isDataSharingDescriptionVisible(): Boolean {
        return dataSharingDescription.element.isCurrentlyVisible
    }

    fun clickDataSharing() {
        btnDataSharing.element.click()
    }


}
