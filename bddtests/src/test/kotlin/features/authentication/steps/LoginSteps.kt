package features.authentication.steps

import mocking.defaults.MockDefaults
import models.Patient
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
    fun using(patient: Patient) {
        loginPage.signIn(patient)
    }

    @Step
    fun asDefault(patient: Patient = MockDefaults.patient,
                  waitForSpinnerToDisappear: Boolean = true) {
        using(patient)
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
        loginPage.clickCreateAccountButton()
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
    fun assertTimeoutBannerIsShown() {
        loginPage.timeoutBannerShouldBeVisible()
    }

    @Step
    fun assertMenuIsNotVisible()
    {
        loginPage.assertMenuIsNotVisible();
    }
}