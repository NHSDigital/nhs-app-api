package pages

fun HybridPageObject.clickOnActionContainingText(actionText: String) {
    getActionPageElement(actionText)
            .assertIsVisible()
            .click()
}

private fun HybridPageObject.getActionPageElement(actionText: String): HybridPageElement {
    val webLocator = "//a[contains(text(),'$actionText')]"
    return HybridPageElement(
        webDesktopLocator = webLocator,
        page = this
    )
}
