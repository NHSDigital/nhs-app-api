package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordDocumentPage : HybridPageObject() {

    val document = HybridPageElement(
        webDesktopLocator = "//div[@class='documentContainer nhsuk-u-margin-top-5']",
        page = this)
}
