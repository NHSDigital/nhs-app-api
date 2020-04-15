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
            page = this
    )
}

fun HybridPageObject.isActionVisible(actionText: String): Boolean {
    var isVisible = false

    getActionPageElement(actionText).actOnTheElement { isVisible = it.isCurrentlyVisible }

    return isVisible
}

/**
 * This wait is used to fix an issue with Chrome bug and will need removing
 * as part of NHSO-8408 when tickets NHSO-8407 and NHSO-8408
 */
const val CHROME_DRIVER_SERVICE_CRASH_DELAY: Long = 130

fun HybridPageObject.avoidChromeWebDriverServiceCrash() {
    Thread.sleep(CHROME_DRIVER_SERVICE_CRASH_DELAY)
}
