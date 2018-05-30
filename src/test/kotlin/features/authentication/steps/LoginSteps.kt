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
    fun asDefault() {
        loginPage.signIn()
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
}