package pages

class NdopPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
            browserLocator = "//p[contains(text(),'token')]",
            androidLocator = null,
            iOSLocator = "//*[@id=\"ndop-token-form\"]",
            page = this
    )

    fun tokenIsDisplayed(): Boolean {
        if (onMobile()){
            return pageTitle.element.isPresent
        } else {
            return pageTitle.element.isDisplayed
        }
    }

}