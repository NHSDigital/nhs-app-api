package features.authentication.steps

import net.thucydides.core.annotations.Step
import org.openqa.selenium.JavascriptExecutor
import pages.notifications.NotificationsPromptPage

open class NotificationSteps {

    lateinit var notificationsPromptPage: NotificationsPromptPage

    @Step
    fun acceptNotifications() {
        notificationsPromptPage.notificationsToggle.assertIsVisible()
        notificationsPromptPage.notificationsToggle.click()

       val executor = notificationsPromptPage.driver as JavascriptExecutor
            executor.executeScript("""
            window.nativeAppCallbacks.notificationsAuthorised(
                '{"trigger":"toggle", "devicePns":"test", "deviceType":"android"}')
        """.trimIndent())

        notificationsPromptPage.continueButton.click()
    }

    @Step
    fun acceptNotificationsButUnauthorisedReturned() {
        notificationsPromptPage.notificationsToggle.assertIsVisible()
        notificationsPromptPage.notificationsToggle.click()

        val executor = notificationsPromptPage.driver as JavascriptExecutor
        executor.executeScript("""
            window.nativeAppCallbacks.notificationsUnauthorised()
        """.trimIndent())
    }

    @Step
    fun dontAcceptNotifications() {
        notificationsPromptPage.notificationsToggle.assertIsVisible()
        notificationsPromptPage.continueButton.click()
    }
}
