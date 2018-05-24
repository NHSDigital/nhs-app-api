package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.At

@At("")
open class HomePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val headerText: String = "Home"

    fun hasWelcomeMessageFor(name: String): Boolean {
        val text = findByXpath("//*[@class='info']/h5").text
        return text.equals(welcomeMessageFor(name))
    }

    private fun welcomeMessageFor(name: String): String {
        return "Welcome, $name"
    }
}
