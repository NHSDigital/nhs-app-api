package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.Header

@DefaultUrl("http://localhost:3000/prescriptions")
open class PrescriptionsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    lateinit var headerBar: Header

    fun isLoaded(): Boolean {
        return headerBar.isVisible("My repeat prescriptions")
    }

    fun isNoPrescriptionsMessageVisible(): Boolean {
        val message = "Looks like you have no repeat prescriptions ordered here."
        return findByXpath("//div[@class='info']//b[contains(.,'$message')]").isVisible
    }
}
