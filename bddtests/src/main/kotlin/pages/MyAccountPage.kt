package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@Suppress("TooManyFunctions")
@DefaultUrl("http://web.local.bitraft.io:3000/account")
class MyAccountPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val aboutUsHeaderXpath = String.format("//div[h2$containsTextXpathSubstring]", "About us")
    private val linkPath = "$aboutUsHeaderXpath/ul/li/a"

    private val usernamePath = "//p//*[@data-sid='user-name']"
    private val dateOfBirthPath = "//p//*[@data-sid='user-date-of-birth']"
    private val nhsNumberPath = "//p//*[@data-sid='user-nhs-number']"

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

    private val usernameText = HybridPageElement(
            browserLocator = usernamePath,
            androidLocator = null,
            page = this
    )

    private val dateOfBirthText = HybridPageElement(
            browserLocator = dateOfBirthPath,
            androidLocator = null,
            page = this
    )

    private val nhsNumberText = HybridPageElement(
            browserLocator = nhsNumberPath,
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

    private val termsOfUseLink = getLink("Terms of use")
    private val privacyPolicyLink = getLink("Privacy policy")
    private val cookiesPolicyLink = getLink("Cookies policy")
    private val openSourceLicensesLink = getLink("Open source licenses")
    private val helpAndSupportLink = getLink("Help and support")

    fun arePersonalDetailsVisible(userName: String, dateOfBirth: String, nhsNumber: String): Boolean {

        var detailsVisible = true

        if(!usernameText.element.isVisible || !dateOfBirthText.element.isVisible || !nhsNumberText.element.isVisible) {
            detailsVisible = false
        }

        var expectedUsername = userName.trim().toLowerCase()
        var expectedDateOfBirth = dateOfBirth.trim().toLowerCase()
        var expectedNhsNumber = nhsNumber.trim().toLowerCase().replace(" ", "")

        var actualUsername = usernameText.element.text.trim().toLowerCase()
        var actualDateOfBirth = dateOfBirthText.element.text.trim().toLowerCase()
        var actualNhsNumber = nhsNumberText.element.text.trim().toLowerCase().replace(" ", "")

        if(expectedUsername != actualUsername) {
            detailsVisible = false
        }

        if(expectedDateOfBirth != actualDateOfBirth) {
            detailsVisible = false
        }

        if(expectedNhsNumber != actualNhsNumber) {
            detailsVisible = false
        }

        return detailsVisible
    }

    fun isSignOutButtonVisible(): Boolean {
        return signOutButton.element.isVisible
    }

    fun isAboutUsHeaderVisible(): Boolean {
        return aboutUsHeader.element.isVisible
    }

    fun assertAllLinksVisible() {
        val expectedLinks = arrayListOf(
                termsOfUseLink,
                privacyPolicyLink,
                cookiesPolicyLink,
                openSourceLicensesLink,
                helpAndSupportLink)

        Assert.assertEquals("Expected Number of Links", expectedLinks.count(), getLink().elements.count())
        expectedLinks.forEach { link -> link.assertSingleElementPresent().assertIsVisible() }
    }

    fun clickTermsOfUseLink() {
        termsOfUseLink.element.click()
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
