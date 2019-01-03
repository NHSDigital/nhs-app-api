package pages

import net.thucydides.core.annotations.DefaultUrl


@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
open class DataSharingPage : HybridPageObject() {

    val btnNext = createBrowserElement("//button[contains(text(), 'Next')]")
    val btnPrevious = createBrowserElement("//button[contains(text(), 'Previous')]")
    val btnStartNow = createBrowserElement("//button[contains(text(), 'Start now')]")

    private val titleOverview = "Overview"
    private val titleDataUse = "Where confidential patient information is used"
    private val titleManageYourChoice = "Where your choice does not apply"

    val linkContentsOverview = contentsLink(titleOverview)
    val linkContentsDataUse = contentsLink(titleDataUse)
    val linkContentsManageYourChoice = contentsLink(titleManageYourChoice)
    val linkDataSharingMoreInfo = createBrowserElement(
            "//a[contains(text(), 'Visit the NHS.UK website')]")

    // Asserts
    private fun titleElement(title: String): HybridPageElement {
        return createBrowserElement("//h1[contains(text(),\"$title\")]")
    }

    fun onOverviewPage() {
        titleElement(titleOverview).assertSingleElementPresent()
    }

    fun onDataUsePage() {
        titleElement(titleDataUse).assertSingleElementPresent()
    }

    fun onManageYourChoicePage() {
        titleElement(titleManageYourChoice).assertSingleElementPresent()
    }

    private fun createBrowserElement(locator: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = locator,
                androidLocator = null,
                page = this
        )
    }

    private fun contentsLink(linkText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//ul[@id='contents']/li/a[contains(text(), \"$linkText\")]",
                androidLocator = null,
                page = this,
                helpfulName = "$'linkText' Link in Contents Element"
        )
    }
}
