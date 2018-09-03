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
