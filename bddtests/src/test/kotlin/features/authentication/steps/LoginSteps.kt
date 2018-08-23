package features.authentication.steps

import config.Config
import mocking.defaults.MockDefaults
import models.Patient
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.LoginPage
import pages.LoginStubPage

open class LoginSteps() {

    lateinit var loginPage: LoginPage
    lateinit var loginStubPage: LoginStubPage

    @Step
    fun checkMySymptoms() {
        loginPage.symptomsButton.element.click()
    }

    @Step
    fun using(patient: Patient) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true") {
            loginStubPage.signIn(patient)
        }
    }

    @Step
    fun asDefault(patient: Patient = MockDefaults.patient) {
        using(patient)
    }

    @Step
    fun assertCreateAccountButtonIsVisible()
    {
        Assert.assertTrue(loginPage.isCreateAccountButtonVisible())
    }

    @Step
    fun clickCreateAccountButton()
    {
        loginPage.loginOrCreateAccountButton.element.click()
    }

    @Step
    fun createAccount(patient: Patient){
        loginPage.createAccount(patient)
    }

    @Step
    fun assertPageIsDisplayed() {
        loginPage.shouldBeDisplayed()
    }

    @Step
    fun assertMenuIsNotVisible()
    {
        loginPage.assertMenuIsNotVisible()
    }
}
