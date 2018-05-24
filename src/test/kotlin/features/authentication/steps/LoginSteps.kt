package features.authentication.steps

import net.thucydides.core.annotations.Step
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
}