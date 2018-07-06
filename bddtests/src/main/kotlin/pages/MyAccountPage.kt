package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/account")
class MyAccountPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val signOutButton = HybridPageElement(
            browserLocator = "//button[@id='signout-button']",
            androidLocator = null,
            page = this
    )

    fun isSignOutButtonVisible() : Boolean {
        return signOutButton.element.isVisible
    }
}