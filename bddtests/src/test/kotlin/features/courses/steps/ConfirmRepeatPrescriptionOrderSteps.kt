package features.courses.steps

import net.thucydides.core.annotations.Step
import org.hamcrest.CoreMatchers.containsString
import org.junit.Assert
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage

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

    @Step
    fun assertSpecialRequest(value: String) {
        Assert.assertThat(confirmRepeatPrescriptionsOrderPage.getSpecialRequest(), containsString(value))
    }

    @Step
    fun assertSpecialRequestNotShown() {
        Assert.assertFalse(confirmRepeatPrescriptionsOrderPage.specialRequestElementIsVisible())
    }

    fun assertErrorSendingOrderShown() {
        Assert.assertTrue(confirmRepeatPrescriptionsOrderPage.errorSendingOrderErrorIsVisible())
    }
}