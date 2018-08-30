package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
open class DataSharingPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val linkContentOverview = createBrowserElement("//span[contains(text(), 'Overview')]")
    private val linkContentManageYourChoice = createBrowserElement("//span[contains(text(), 'Manage your choice')]")
    private val btnNext = createBrowserElement("//button[contains(text(), 'Next')]")
    private val btnPrevious = createBrowserElement("//button[contains(text(), 'Previous')]")
    private val btnStartNow = createBrowserElement("//button[contains(text(),'Start Now')]")
    private val titleOverview = createBrowserElement("//h1[contains(text(),'Overview')]")
    private val titleManageYourChoice = createBrowserElement("//h1[contains(text(),'Manage your choice')]")

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

    fun isStartNowVisible(): Boolean {
        return btnStartNow.element.isCurrentlyVisible
    }

    private fun createBrowserElement(locator: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = locator,
                androidLocator = null,
                page = this
        )
    }
}
