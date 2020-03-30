package pages.ndop

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent
import pages.sharedElements.Pagination
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/data-sharing")
abstract class DataSharingPage : HybridPageObject() {

    protected val dataSharingSubheader = "Choose if data from your health records is shared for research and planning"
    protected val overviewPageTitle = "Overview"
    protected val confidentialDataPageTitle = "How confidential patient information is used"
    protected val choiceApplyPageTitle = "When your choice does not apply"
    protected val makeChoicePageTitle = "Make your choice"
    protected val contentsTitle = "Contents"

    abstract val pageTitle: String
    abstract val nextPageTitle: String?
    abstract val previousPageTitle: String?
    abstract val expectedPageStructure: ExpectedPageStructure

    fun assertContent() {
        expectedPageStructure.assert(this)
    }

    fun previousButton(): Pagination {return Pagination("Previous", this)}
    fun nextButton (): Pagination {return Pagination("Next", this)}

    fun assertDisplayed() {
        val title = HybridPageElement(
                webDesktopLocator = "//h1[normalize-space(text())='$pageTitle']",
                androidLocator = null,
                page = this
        )
        title.assertSingleElementPresent()
    }

    fun contentsLink(linkText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//ol/li/a[normalize-space(text())='$linkText']",
                androidLocator = null,
                page = this,
                helpfulName = "'$linkText' Link in Contents Element"
        )
    }
}