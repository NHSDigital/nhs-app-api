package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
open class DataSharingPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val btnNext = createBrowserElement("//button[contains(text(), 'Next')]")
    private val btnPrevious = createBrowserElement("//button[contains(text(), 'Previous')]")
    private val btnStartNow = createBrowserElement("//button[contains(text(),'Start now')]")
    private val titleOverview = createBrowserElement("//h1[contains(text(),'Overview')]")
    private val titleBenefits = createBrowserElement("//h1[contains(text(),'Benefits of data sharing')]")
    private val titleDataUse = createBrowserElement("//h1[contains(text(),'How your data is used')]")
    private val titleWhereOptOutDoesntApply = createBrowserElement("//h1[contains(text(),\"Where an opt-out doesn't apply\")]")
    private val titleManageYourChoice = createBrowserElement("//h1[contains(text(),'Manage your choice')]")
    private val linkContentsOverview = createBrowserElement("//ul[@id='contents']/li/a[contains(text(), 'Overview')]")
    private val linkContentsBenefits = createBrowserElement("//ul[@id='contents']/li/a[contains(text(), 'Benefits of data sharing')]")
    private val linkContentsDataUse = createBrowserElement("//ul[@id='contents']/li/a[contains(text(), 'How your data is used')]")
    private val linkContentsWhereOptOutDoesntApply = createBrowserElement("//ul[@id='contents']/li/a[contains(text(), \"Where an opt-out doesn't apply\")]")
    private val linkContentsManageYourChoice = createBrowserElement("//ul[@id='contents']/li/a[contains(text(), 'Manage your choice')]")
    private val linkManageYourChoice = createBrowserElement("//a[@id='manage-choice-link'][contains(text(), 'Manage your choice')]")
    private val linkDataSharingMoreInfo = createBrowserElement("//a[contains(text(), 'NHS website')]")

    // Actions

    fun clickManageYourChoice() {
        linkManageYourChoice.element.click()
    }

    fun clickDataSharingMoreInfoLink() {
        linkDataSharingMoreInfo.element.click()
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

    fun clickOverviewContentsLink() {
        linkContentsOverview.element.click()
    }

    fun clickBenefitsContentsLink() {
        linkContentsBenefits.element.click()
    }

    fun clickDataUseContentsLink(){
        linkContentsDataUse.element.click()
    }

    fun clickWhereOptOutDoesntApplyContentsLink(){
        linkContentsWhereOptOutDoesntApply.element.click()
    }

    fun clickManageYourChoiceContentsLink() {
        linkContentsManageYourChoice.element.click()
    }

    // Asserts

    fun onOverviewPage(): Boolean {
        return titleOverview.element.isCurrentlyVisible
    }

    fun onBenefitsPage(): Boolean {
        return titleBenefits.element.isCurrentlyVisible
    }

    fun onDataUsePage(): Boolean {
        return titleDataUse.element.isCurrentlyVisible
    }

    fun onWhereOptOutDoesntApplyPage(): Boolean {
        return titleWhereOptOutDoesntApply.element.isCurrentlyVisible
    }

    fun onManageYourChoicePage(): Boolean {
        return titleManageYourChoice.element.isCurrentlyVisible
    }

    private fun createBrowserElement(locator: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = locator,
                androidLocator = null,
                page = this
        )
    }

}
