package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
open class DataSharingPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val linkContentOverview = HybridPageElement(
            browserLocator = "//span[contains(text(), 'Overview')]",
            androidLocator = null,
            page = this
    )

    val linkContentManageYourChoice = HybridPageElement(
            browserLocator = "//span[contains(text(), 'Manage your choice')]",
            androidLocator = null,
            page = this
    )

    val btnNext = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Next')]",
            androidLocator = null,
            page = this
    )

    val btnPrevious = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Previous')]",
            androidLocator = null,
            page = this
    )

    val btnStartNow = HybridPageElement(
            browserLocator = "//button[contains(text(),'Start Now')]",
            androidLocator = null,
            page = this
    )

    val titleOverview = HybridPageElement(
            browserLocator = "//h1[contains(text(),'Overview')]",
            androidLocator = null,
            page = this
    )

    val titleManageYourChoice = HybridPageElement(
            browserLocator = "//h1[contains(text(),'Manage your choice')]",
            androidLocator = null,
            page = this
    )

    val ndopTestDescription = HybridPageElement(
            browserLocator = "//label[contains(text(),'Linked to Ndop OK')]",
            androidLocator = null,
            page = this
    )

    // Actions

    fun clickOverview() {
        linkContentOverview.element.click()
    }

    fun clickManageYourChoice() {
        linkContentManageYourChoice.element.click()
    }

    fun clickNext() {
        btnNext.element.click()
    }

    fun clickPrevious() {
        btnPrevious.element.click()
    }

    fun clickStartNow() {
        btnStartNow.element.click()
    }

    // Asserts

    fun onOverviewPage(): Boolean {
        return titleOverview.element.isCurrentlyVisible
    }

    fun onManageYourChoicePage(): Boolean {
        return titleManageYourChoice.element.isCurrentlyVisible
    }

    fun isNextVisible(): Boolean {
        return btnNext.element.isCurrentlyVisible
    }

    fun isPreviousVisible(): Boolean {
        return btnPrevious.element.isCurrentlyVisible
    }

    fun isStartNowVisible(): Boolean {
        return btnStartNow.element.isCurrentlyVisible
    }

    fun isNdopTestTextIsVisible(): Boolean {
        return ndopTestDescription.element.isCurrentlyVisible
    }
}
