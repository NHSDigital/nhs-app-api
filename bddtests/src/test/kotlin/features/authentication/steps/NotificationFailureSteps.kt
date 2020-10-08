package features.authentication.steps

import net.thucydides.core.annotations.Step
import pages.assertIsVisible
import pages.notifications.NotificationsPromptFailurePage

open class NotificationFailureSteps {

    lateinit var notificationsPromptFailurePage: NotificationsPromptFailurePage

    @Step
    fun continueToHome() {
        notificationsPromptFailurePage.continueButton.assertIsVisible()
        notificationsPromptFailurePage.continueButton.click()
    }
}
