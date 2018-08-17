package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/account")
class MyAccountPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

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

    val termsAndConditionsLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Terms and conditions')]",
            androidLocator = null,
            page = this
    )

    val privacyPolicyLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Privacy policy')]",
            androidLocator = null,
            page = this
    )

    val cookiesPolicyLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Cookies policy')]",
            androidLocator = null,
            page = this
    )

    val openSourceLicensesLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Open source licenses')]",
            androidLocator = null,
            page = this
    )

    val helpAndSupportLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Help and support')]",
            androidLocator = null,
            page = this
    )


    fun isSignOutButtonVisible() : Boolean {
        return signOutButton.element.isVisible
    }

    fun isAboutUsHeaderVisible() : Boolean {
        return aboutUsHeader.element.isVisible
    }

    fun isTermsAndConditionsLinkVisible() : Boolean {
        return termsAndConditionsLink.element.isVisible
    }

    fun isPrivacyPolicyLinkVisible() : Boolean {
        return privacyPolicyLink.element.isVisible
    }

    fun isCookiesPolicyLinkVisible() : Boolean {
        return cookiesPolicyLink.element.isVisible
    }

    fun isOpenSourceLicensesLinkVisible() : Boolean {
        return openSourceLicensesLink.element.isVisible
    }

    fun isHelpAndSupportLinkVisible() : Boolean {
        return helpAndSupportLink.element.isVisible
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
