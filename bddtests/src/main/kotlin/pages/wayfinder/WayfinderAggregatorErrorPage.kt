package pages.wayfinder

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/wayfinder")
open class WayfinderAggregatorErrorPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Cannot view or manage referrals and appointments\")]",
        page = this
    )

    private val tryAgainText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(), \"Try again. If the problem continues and you need to access your " +
            "referrals or appointments now you may be able to do this using other services\")]",
        page = this
    )

    private val tryAgainButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(),\"Try again\")]",
        page = this
    )

    private fun contactUsLink(prefix: String) = HybridPageElement(
        webDesktopLocator = "//a[@id='contact-us-link']" +
            "[starts-with(@href, 'http://stubs.local.bitraft.io:8080" +
            "/external/nhsuk/nhs-app-contact-us?errorcode=${prefix}')]",
        page = this
    )

    private val underMinimumAgeText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(), \"If you're aged 15 or under you may be able to access" +
            " your referrals and appointments using other services.\")]",
        page = this
    )

    private val otherServicesHeader = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(), \"Other services\")]",
        page = this
    )

    private fun assertContactUsLinkText(prefix: String) {
        val regex = "Contact us if the problem continues, quoting error code ${prefix}\\w{4}".toRegex()

        Assert.assertTrue("Expected 'Contact us link' text to match '${regex.toPattern()}'",
            regex.matches(contactUsLink(prefix).text))
    }

    fun assertIsDisplayedWithPrefix(prefix: String) {
        pageTitle.assertIsVisible()
        tryAgainText.assertIsVisible()
        tryAgainButton.assertIsVisible()
        assertContactUsLinkText(prefix)
        otherServicesHeader.assertIsVisible()
    }

    fun assertIsDisplayedWithUnderMinimumAgeError() {
        pageTitle.assertIsVisible()
        underMinimumAgeText.assertIsVisible()
        otherServicesHeader.assertIsVisible()
    }

    fun clickTryAgain() {
        tryAgainButton.click()
    }

    fun clickContactUsLinkWithPrefix(prefix: String) {
        contactUsLink(prefix).click()
    }
}
