package pages


fun HybridPageObject.clickOnActionContainingText(actionText: String) {
    getActionPageElement(actionText)
            .assertIsVisible()
            .click()
}

private fun HybridPageObject.getActionPageElement(actionText: String): HybridPageElement {
    val webLocator = "//a[contains(text(),'$actionText')]"
    val nativeLocator = "//button[contains(text(),'$actionText')]"
    return HybridPageElement(
            webDesktopLocator = webLocator,
            iOSLocator = nativeLocator,
            androidLocator = nativeLocator,
            page = this,
            timeToWaitForElement = 30
    )
}

fun HybridPageObject.isActionVisible(actionText: String): Boolean {
    var isVisible = false

    getActionPageElement(actionText).actOnTheElement { isVisible = it.isCurrentlyVisible }

    return isVisible
}


