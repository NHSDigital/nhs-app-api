package pages.accountAndSettings
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/manage-notifications/more-than-one-device")
class MoreThanOneDevicePage : HybridPageObject() {

    fun assertDisplayed() {
        title.waitForElement()
    }

    private val title by lazy {
        HybridPageElement(
                "//h1[normalize-space(text())='Managing notifications if you have more than one device']",
                this,
                helpfulName = "header"
        )
    }
}

