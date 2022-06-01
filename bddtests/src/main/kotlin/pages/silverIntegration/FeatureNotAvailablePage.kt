package pages.silverIntegration

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/silver-integration/feature-not-available")
open class FeatureNotAvailablePage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    val heading = HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='content']/h1",
            helpfulName = "error heading",
            page = this
    )

    val youDoNotHaveAccess =  HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='content']/span",
            helpfulName = "content",
            page = this
    )

    val goToNHSAppHome =  HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='go-to-nhs-app-home']/a",
            helpfulName = "go to nhs app home link",
            page = this
    )

    fun isLoaded(header: String) {
        webHeader.waitForPageHeaderText(header)

        Assert.assertEquals(
                header,
                heading.text)

        Assert.assertEquals(
                "This service is provided by Patients Know Best (a health service provider), " +
                         "which you do not have access to on the NHS App.",
                youDoNotHaveAccess.text)
                
        Assert.assertEquals(
                "Go to NHS App homepage",
                goToNHSAppHome.text)
    }
}
