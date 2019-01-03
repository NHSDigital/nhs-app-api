package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordNoAccessPage : HybridPageObject() {

    val txtNoAccess = HybridPageElement(
            webDesktopLocator = "",
            androidLocator = null,
            page = this
    )

    val txtGPTxt = HybridPageElement(
            webDesktopLocator = "",
            androidLocator = null,
            page = this
    )

    fun isOnNoAccessPage(): Boolean {
        return txtNoAccess.element.isCurrentlyVisible
    }

    fun getNoAccessText(): String {
        return txtNoAccess.element.text
    }

    fun getContactGPText(): String {
        return txtGPTxt.element.text
    }
}
