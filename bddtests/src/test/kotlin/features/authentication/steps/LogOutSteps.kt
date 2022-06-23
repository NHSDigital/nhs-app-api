package features.authentication.steps

import config.Config
import features.sharedSteps.BrowserSteps
import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.loggedOut.LogOutPage
import pages.loggedOut.LoginStubPage
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

open class LogOutSteps {

    lateinit var logOutPage: LogOutPage

    @Steps
    lateinit var browser: BrowserSteps
    private lateinit var loginStubPage: LoginStubPage

    @Step
    fun using(patient: Patient) {
        stubbedLoginAndResetScripts(patient)
    }

    private fun stubbedLoginAndResetScripts (
        patient: Patient
    ) {
        logOutPage.signIn()
        if(Config.instance.autoLogin != "true" && !OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginStubPage.signIn(patient)
        }
    }


}
