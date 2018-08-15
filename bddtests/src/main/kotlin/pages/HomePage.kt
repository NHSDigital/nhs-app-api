package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/")
open class HomePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val headerText: String = "Home"

    fun hasWelcomeMessageFor(name: String): Boolean {
        val text = findByXpath("//*[@data-purpose='greeting']").text
        return text == welcomeMessageFor(name)
    }

    private fun welcomeMessageFor(name: String): String {
        return "Welcome, $name"
    }

}
