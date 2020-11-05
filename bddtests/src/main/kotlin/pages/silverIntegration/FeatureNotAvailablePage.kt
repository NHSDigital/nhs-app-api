package pages.silverIntegration

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/silver-integration/feature-not-available")
open class FeatureNotAvailablePage : HybridPageObject() {

    val heading = HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='error-heading']",
            helpfulName = "error heading",
            page = this
    )

    val contactYourGpSurgery =  HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='contact']",
            helpfulName = "contact",
            page = this
    )

    val goToNHSAppHome =  HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='go-to-nhs-app-home']/a",
            helpfulName = "go to nhs app home link",
            page = this
    )

    private lateinit var headerNative: HeaderNative

    fun isLoaded(header: String) {
        headerNative.waitForPageHeaderText(header)
        Assert.assertEquals(
                "This service is not available",
                heading.text)

        Assert.assertEquals(
                "If you need to access this service, contact your GP surgery for more information.",
                contactYourGpSurgery.text)

        Assert.assertEquals(
                "Go to NHS App homepage",
                goToNHSAppHome.text)
    }
}
