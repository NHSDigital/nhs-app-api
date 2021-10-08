package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordDocumentPage : HybridPageObject() {

    val document = HybridPageElement(
        webDesktopLocator = "//div[@id='document']",
        page = this)
}
