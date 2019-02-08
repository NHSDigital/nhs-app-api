package features.authentication.steps

import config.Config
import models.Patient
import net.thucydides.core.annotations.Step
import pages.loggedOut.LoginPage
import pages.loggedOut.LoginStubPage
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

open class LoginSteps {

    lateinit var loginPage: LoginPage
    private lateinit var loginStubPage: LoginStubPage

    @Step
    fun using(patient: Patient) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true" && !OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginStubPage.signIn(patient)
        }
    }
}
