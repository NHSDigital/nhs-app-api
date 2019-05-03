package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@DefaultUrl("http://web.local.bitraft.io:3000/account")
class MyAccountPage : HybridPageObject() {

    private val aboutUsHeaderXpath = String.format("//div[h2$containsTextXpathSubstring]", "About us")
    private val linkPath = "$aboutUsHeaderXpath/ul/li/a"

    private val usernamePath = "//p//*[@data-sid='user-name']"
    private val dateOfBirthPath = "//p//*[@data-sid='user-date-of-birth']"
    private val nhsNumberPath = "//p//*[@data-sid='user-nhs-number']"

    val signOutButton = HybridPageElement(
            webDesktopLocator = "//a[@id='account-logout']",
            iOSLocator="//button[@id='signout-button']",
            androidLocator = "//button[@id='signout-button']",
            page = this
    )

    private val aboutUsHeader = HybridPageElement(
            webDesktopLocator = aboutUsHeaderXpath,
            androidLocator = null,
            page = this
    )

    private val usernameText = HybridPageElement(
            webDesktopLocator = usernamePath,
            androidLocator = null,
            page = this
    )

    private val dateOfBirthText = HybridPageElement(
            webDesktopLocator = dateOfBirthPath,
            androidLocator = null,
            page = this
    )

    private val nhsNumberText = HybridPageElement(
            webDesktopLocator = nhsNumberPath,
            androidLocator = null,
            page = this
    )

    private fun getLink(text: String = ""): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$linkPath${String.format(containsTextXpathSubstring, text)}",
                androidLocator = null,
                page = this,
                helpfulName = "$text Link")
    }

    val termsOfUseLink = getLink("Terms of use")
    val privacyPolicyLink = getLink("Privacy policy")
    val cookiesPolicyLink = getLink("Cookies policy")
    val openSourceLicencesLink = getLink("Open source licences")
    val helpAndSupportLink = getLink("Help and support")
    val accessibilityStatementLink = getLink("Accessibility statement")

    fun assertPersonalDetailsVisible(expectedUsername: String,
                                     expectedDateOfBirth: String,
                                     expectedNhsNumber: String) {
        usernameText.assertIsVisible()
        dateOfBirthText.assertIsVisible()
        nhsNumberText.assertIsVisible()

        val actualUsername = usernameText.text.trim().toLowerCase()
        val actualDateOfBirth = dateOfBirthText.text.trim().toLowerCase()
        val actualNhsNumber = nhsNumberText.text.trim()

        Assert.assertEquals("Username", expectedUsername.toLowerCase(), actualUsername)
        Assert.assertEquals("Date Of Birth", expectedDateOfBirth.toLowerCase(), actualDateOfBirth)
        Assert.assertEquals("NHS Number", expectedNhsNumber, actualNhsNumber)
    }

    fun isAboutUsHeaderVisible(): Boolean {
        return aboutUsHeader.isVisible
    }

    fun assertAllLinksVisible() {
        val expectedLinks = arrayListOf(
                termsOfUseLink,
                privacyPolicyLink,
                cookiesPolicyLink,
                openSourceLicencesLink,
                helpAndSupportLink,
                accessibilityStatementLink)

        Assert.assertEquals("Expected Number of Links", expectedLinks.count(), getLink().elements.count())
        expectedLinks.forEach { link -> link.assertSingleElementPresent().assertIsVisible() }
    }
}
