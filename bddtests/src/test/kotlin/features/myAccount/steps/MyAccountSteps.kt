package features.myAccount.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.MyAccountPage

open class MyAccountSteps {

    lateinit var myAccountPage: MyAccountPage

    @Step
    fun signOut() {
        myAccountPage.signOutButton.element.click()
    }

    @Step
    fun assertSignoutButtonVisible() {
        Assert.assertTrue("Sign out button not visible, expected to be visible", myAccountPage.isSignOutButtonVisible())
    }

    @Step
    fun assertAboutUsHeaderVisible() {
        Assert.assertTrue("About us header not visible, expected to be visible", myAccountPage.isAboutUsHeaderVisible())
    }

    @Step
    fun assertTermsAndConditionsLinkVisible() {
        Assert.assertTrue("terms and conditions link not visible, expected to be visible", myAccountPage.isTermsAndConditionsLinkVisible())
    }

    @Step
    fun assertPrivacyPolicyLinkVisible() {
        Assert.assertTrue("Privacy policy link not visible, expected to be visible", myAccountPage.isPrivacyPolicyLinkVisible())
    }

    @Step
    fun assertCookiesPolicyLinkVisible() {
        Assert.assertTrue("Cookies policy link not visible, expected to be visible", myAccountPage.isCookiesPolicyLinkVisible())
    }

    @Step
    fun assertOpenSourceLicensesLinkVisible() {
        Assert.assertTrue("Open source licenses link not visible, expected to be visible", myAccountPage.isOpenSourceLicensesLinkVisible())
    }

    @Step
    fun assertHelpAndSupportLinkVisible() {
        Assert.assertTrue("Help and support link not visible, expected to be visible", myAccountPage.isHelpAndSupportLinkVisible())
    }

    @Step
    fun goToTermsAndConditions() {
        myAccountPage.clickTermsAndConditionsLink()
    }

    @Step
    fun goToPrivacyPolicy() {
        myAccountPage.clickPrivacyPolicyLink()
    }

    @Step
    fun goToCookiesPolicy() {
        myAccountPage.clickCookiesPolicyLink()
    }

    @Step
    fun goToOpenSourceLicenses() {
        myAccountPage.clickOpenSourceLicensesLink()
    }

    @Step
    fun goToHelpAndSupport() {
        myAccountPage.clickHelpAndSupportLink()
    }
}
