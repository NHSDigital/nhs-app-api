package features.authentication.steps

import net.thucydides.core.annotations.Step
import org.openqa.selenium.JavascriptExecutor
import pages.notifications.NotificationsPromptPage

open class NotificationSteps {

    lateinit var notificationsPromptPage: NotificationsPromptPage

    @Step
    fun allowNotifications() {
       val executor = notificationsPromptPage.driver as JavascriptExecutor
            executor.executeScript("""
            window.nativeAppCallbacks.notificationsAuthorised(
                '{"trigger":"toggle", "devicePns":"test", "deviceType":"android"}')
        """.trimIndent())
    }

    @Step
    fun denyNotifications() {
        val executor = notificationsPromptPage.driver as JavascriptExecutor
        executor.executeScript("""
            window.nativeAppCallbacks.notificationsUnauthorised()
        """.trimIndent())
    }
}
