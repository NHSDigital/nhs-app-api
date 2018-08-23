package pages

import net.thucydides.core.annotations.DefaultUrl


@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
open class DataSharingPage : HybridPageObject() {

    val btnNext = createBrowserElement("//button[contains(text(), 'Next')]")
    val btnPrevious = createBrowserElement("//button[contains(text(), 'Previous')]")
    val btnStartNow = createBrowserElement("//button[contains(text(), 'Start now')]")

    private val titleOverview = "Overview"
    private val titleBenefits = "Benefits of data sharing"
    private val titleDataUse = "How your data is used"
    private val titleWhereOptOutDoesntApply = "Where an opt-out doesn't apply"
    private val titleManageYourChoice = "Manage your choice"

    val linkContentsOverview = contentsLink(titleOverview)
    val linkContentsBenefits = contentsLink(titleBenefits)
    val linkContentsDataUse = contentsLink(titleDataUse)
    val linkContentsWhereOptOutDoesntApply = contentsLink(titleWhereOptOutDoesntApply)
    val linkContentsManageYourChoice = contentsLink(titleManageYourChoice)

    val linkManageYourChoice = createBrowserElement(
            "//a[@id='manage-choice-link'][contains(text(), 'Manage your choice')]")
    val linkDataSharingMoreInfo = createBrowserElement(
            "//a[contains(text(), 'NHS website')]")

    // Asserts
    private fun titleElement(title: String): HybridPageElement {
        return createBrowserElement("//h1[contains(text(),\"$title\")]")
    }

    fun onOverviewPage() {
        titleElement(titleOverview).assertSingleElementPresent()
    }

    fun onBenefitsPage() {
        titleElement(titleBenefits).assertSingleElementPresent()
    }

    fun onDataUsePage() {
        titleElement(titleDataUse).assertSingleElementPresent()
    }

    fun onWhereOptOutDoesntApplyPage() {
        titleElement(titleWhereOptOutDoesntApply).assertSingleElementPresent()
    }

    fun onManageYourChoicePage() {
        titleElement(titleManageYourChoice).assertSingleElementPresent()
    }

    private fun createBrowserElement(locator: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = locator,
                androidLocator = null,
                page = this
        )
    }

    private fun contentsLink(linkText: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = "//ul[@id='contents']/li/a[contains(text(), \"$linkText\")]",
                androidLocator = null,
                page = this,
                helpfulName = "$'linkText' Link in Contents Element"
        )
    }
}
