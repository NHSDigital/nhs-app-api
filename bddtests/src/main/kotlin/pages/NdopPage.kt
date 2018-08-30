package pages

class NdopPage : HybridPageObject(Companion.PageType.WEBVIEW_BROWSER) {

    val pageTitle = HybridPageElement(
        browserLocator = "//p[contains(text(),'token')]",
        androidLocator = null,
        page = this
    )

    fun tokenIsDisplayed(): Boolean {
        return pageTitle.element.isDisplayed
    }

}