package features.courses.steps

import net.thucydides.core.annotations.Step
import pages.ConfirmRepeatPrescriptionsOrderPage

open class ConfirmRepeatPrescriptionOrderSteps {

    lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage

    @Step
    open fun isLoaded() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
    }

    @Step
    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmRepeatPrescriptionsOrderPage.clickConfirmAndOrderRepeatPrescriptionButton()
    }

    @Step
    fun clickChangeThisPrescriptionButton() {
        confirmRepeatPrescriptionsOrderPage.clickChangeThisPrescriptionButton()
    }
}