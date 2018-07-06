package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordNoAccessPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val txtNoAccess = HybridPageElement(
            browserLocator = "",
            androidLocator = null,
            page = this
    )

    val txtGPTxt = HybridPageElement(
            browserLocator = "",
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
