package features.sharedSteps

import net.thucydides.core.annotations.Step
import org.openqa.selenium.JavascriptExecutor
import pages.HybridPageObject

open class BiometricSteps {

    private lateinit var genericPage: HybridPageObject

    @Step
    fun setBiometricType(biometricReference: String) {
        triggerWindowDispatch(
                "loginSettings/biometricSpec",
                "{ biometricTypeReference:'${biometricReference}', enabled: false}"
        )
    }

    @Step
    fun setBiometricCompletionResult(action: String, outcome: String, errorCode: String) {
        triggerWindowDispatch(
                "loginSettings/biometricCompletion",
                "{ action:'${action}', outcome:'${outcome}', errorCode:'${errorCode}' }"
        )
    }

    @Step
    fun triggerBiometricLoginError() {
        triggerWindowDispatch(
                "login/handleBiometricLoginFailure", "''"
        )
    }

    private fun triggerWindowDispatch(event: String, arg: String) {
        val jsExecutor = genericPage.driver as JavascriptExecutor
        jsExecutor.executeScript("""
            window.appEvent({ event: "${event}", payload: ${arg} });
        """.trimIndent())
    }
}
