package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/data-sharing")
open class DataSharingPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val btnStartNow = HybridPageElement(
            browserLocator = "//button[contains(text(),'Start Now')]",
            androidLocator = null,
            page = this
    )

    val ndopTestDescription = HybridPageElement(
            browserLocator = "//label[contains(text(),'Linked to Ndop OK')]",
            androidLocator = null,
            page = this
    )

    fun isStartNowVisible(): Boolean {
        return btnStartNow.element.isCurrentlyVisible
    }

    fun clickStartNow() {
        btnStartNow.element.click()
    }

    fun isNdopTestTextIsVisible(): Boolean {
        return ndopTestDescription.element.isCurrentlyVisible
    }
}
