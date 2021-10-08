package pages.legalAndCookies

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/legal-and-cookies/")
class LegalAndCookiesPage : HybridPageObject() {

    var manageCookiesLink =
        getElement("//h2[contains(text(), 'Manage cookies')]")
    var termsOfUseLink =
        getElement("//h2[contains(text(), 'Terms of use')]")
    var privacyPolicyLink =
        getElement("//h2[contains(text(), 'Privacy policy')]")
    var accessibilityStatementLink =
        getElement("//h2[contains(text(), 'Accessibility statement')]")
    var openSourceLicencesLink =
        getElement("//h2[contains(text(), 'Open source licences')]")

    fun assertDisplayed() {
        title.waitForElement()
        manageCookiesLink.assertIsVisible()
        termsOfUseLink.assertIsVisible()
        privacyPolicyLink.assertIsVisible()
        accessibilityStatementLink.assertIsVisible()
        openSourceLicencesLink.assertIsVisible()
    }

    val clickManageCookies by lazy {
        manageCookiesLink.click()
    }

    private val title by lazy {
        HybridPageElement(
            "//h1[normalize-space(text())='Legal and cookies']",
            this,
            helpfulName = "header")
    }
}

