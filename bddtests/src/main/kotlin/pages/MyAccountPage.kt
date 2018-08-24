package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@Suppress("TooManyFunctions")
@DefaultUrl("http://localhost:3000/account")
class MyAccountPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val signOutButton = HybridPageElement(
            browserLocator = "//*[@id='btn_floating']",
            androidLocator = null,
            page = this
    )

    val aboutUsHeader = HybridPageElement(
            browserLocator = "//h2[contains(text(),'About us')]",
            androidLocator = null,
            page = this
    )

    var linkPath = "//div[h2[contains(text(),'About us')]]/ul/li/a"

    private val links = HybridPageElement(
                browserLocator = linkPath,
                androidLocator = null,
                page = this)

    private fun getLink(text:String): HybridPageElement{
        return HybridPageElement(
                browserLocator = linkPath,
                androidLocator = null,
                page = this,
                helpfulName = "$text Link").containingText(text)
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

        Assert.assertEquals("Expected Number of Links", expectedLinks.count(), links.elements.count())
        expectedLinks.forEach { link -> link.assertSingleElementPresent().assertIsVisible() }
    }
    
    fun clickTermsAndConditionsLink() {
        findByXpath("//*[@id='btn_terms']").click()
    }

    fun clickPrivacyPolicyLink() {
        findByXpath("//*[@id='btn_privacy']").click()
    }

    fun clickCookiesPolicyLink() {
        findByXpath("//*[@id='btn_cookies']").click()
    }

    fun clickOpenSourceLicensesLink() {
        findByXpath("//*[@id='btn_openSource']").click()
    }

    fun clickHelpAndSupportLink() {
        helpAndSupportLink.element.click()
    }
}
