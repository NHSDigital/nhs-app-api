package features.authentication.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.LoginPage

open class LoginSteps {

    lateinit var loginPage: LoginPage

    @Step
    fun checkMySymptoms() {
        loginPage.checkMySymptoms()
    }

    @Step
    fun asDefault(waitForSpinnerToDisappear: Boolean = true) {
        loginPage.signIn()
        if (waitForSpinnerToDisappear) loginPage.waitForSpinnerToDisappear()
    }

    @Step
    fun assertCreateAccountButtonIsVisible()
    {
        Assert.assertTrue(loginPage.isCreateAccountButtonVisible())
    }

    @Step
    fun clickCreateAccountButton()
    {
        loginPage.clickCreateAccountButton();
    }

    @Step
    fun createAccount(){
        loginPage.createAccount();
    }

    @Step
    fun assertPageIsDisplayed() {
        loginPage.shouldBeDisplayed()
    }

    @Step
    fun assertTimeoutBannerIsShown() {
        loginPage.timeoutBannerShouldBeVisible()
    }

    @Step
    fun assertMenuIsNotVisible()
    {
        loginPage.assertMenuIsNotVisible();
    }
}