package pages.account

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isVisible

@DefaultUrl("http://web.local.bitraft.io:3000/account/login-settings")
class LoginSettingsPage : HybridPageObject() {

    private val titleLocator = "//h1[normalize-space(text())='Login options']"

    val title by lazy {
        HybridPageElement(
                titleLocator,
                titleLocator,
                null,
                null,
                this,
                helpfulName = "header")
    }

    fun assertDisplayed() {
        Assert.assertTrue(title.isVisible)
    }
}

