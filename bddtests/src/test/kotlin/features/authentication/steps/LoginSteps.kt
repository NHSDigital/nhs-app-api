package features.authentication.steps

import config.Config
import models.Patient
import net.thucydides.core.annotations.Step
import pages.LoginPage
import pages.LoginStubPage

open class LoginSteps {

    lateinit var loginPage: LoginPage
    private lateinit var loginStubPage: LoginStubPage

    @Step
    fun using(patient: Patient) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true") {
            loginStubPage.signIn(patient)
        }
    }
}
