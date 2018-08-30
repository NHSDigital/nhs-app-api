package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@Suppress("TooManyFunctions")
@DefaultUrl("http://web.local.bitraft.io:3000/account")
class MyAccountPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val aboutUsHeaderXpath = String.format("//div[h2$containsTextXpathSubstring]", "About us")
    private val linkPath = "$aboutUsHeaderXpath/ul/li/a"

    val signOutButton = HybridPageElement(
            browserLocator = "//button[@id='signout-button']",
            androidLocator = null,
            page = this
    )

    private val aboutUsHeader = HybridPageElement(
            browserLocator = aboutUsHeaderXpath,
            androidLocator = null,
            page = this
    )

    private fun getLink(text: String = ""): HybridPageElement {
        return HybridPageElement(
                browserLocator = "$linkPath${String.format(containsTextXpathSubstring, text)}",
                androidLocator = null,
                page = this,
                helpfulName = "$text Link")
    }

    private val termsAndConditionsLink = getLink("Terms of use")
    private val privacyPolicyLink = getLink("Privacy policy")
    private val cookiesPolicyLink = getLink("Cookies policy")
    private val openSourceLicensesLink = getLink("Open source licenses")
    private val helpAndSupportLink = getLink("Help and support")

    fun isSignOutButtonVisible(): Boolean {
        return signOutButton.element.isVisible
    }

    fun isAboutUsHeaderVisible(): Boolean {
        return aboutUsHeader.element.isVisible
    }

    fun assertAllLinksVisible() {
        val expectedLinks = arrayListOf(
                termsAndConditionsLink,
                privacyPolicyLink,
                cookiesPolicyLink,
                openSourceLicensesLink,
                helpAndSupportLink)

        Assert.assertEquals("Expected Number of Links", expectedLinks.count(), getLink().elements.count())
        expectedLinks.forEach { link -> link.assertSingleElementPresent().assertIsVisible() }
    }

    fun clickTermsAndConditionsLink() {
        termsAndConditionsLink.element.click()
    }

    fun clickPrivacyPolicyLink() {
        privacyPolicyLink.element.click()
    }

    fun clickCookiesPolicyLink() {
        cookiesPolicyLink.element.click()
    }

    fun clickOpenSourceLicensesLink() {
        openSourceLicensesLink.element.click()
    }

    fun clickHelpAndSupportLink() {
        helpAndSupportLink.element.click()
    }
}
