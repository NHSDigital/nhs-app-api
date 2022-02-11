package pages

fun HybridPageObject.clickOnActionContainingText(actionText: String) {
    getActionPageElement(actionText)
            .assertIsVisible()
            .click()
}

fun HybridPageObject.clickOnActionWithId(idText: String) {
    getActionPageElementUsingId(idText)
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

private fun HybridPageObject.getActionPageElementUsingId(idText: String): HybridPageElement {
    val webLocator = "//a[@id='$idText']"
    return HybridPageElement(
        webDesktopLocator = webLocator,
        page = this
    )
}
