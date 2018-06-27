package features.courses.steps

import net.thucydides.core.annotations.Step
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import org.hamcrest.CoreMatchers.containsString
import org.junit.Assert

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
}