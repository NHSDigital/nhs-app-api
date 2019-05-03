package features.courses.steps

import net.thucydides.core.annotations.Step
import org.hamcrest.CoreMatchers.containsString
import org.junit.Assert
import pages.ErrorPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.text

open class ConfirmRepeatPrescriptionOrderSteps {

    lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage
    lateinit var errorPage: ErrorPage

    @Step
    open fun isLoaded() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
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

    fun assertMedicationOrderedWithinTheLast30DaysErrorShown() {
        val expectedHeader = "We cannot complete this order"
        val expectedSubHeader = "You previously ordered at least one of these medications in the last 30 days."
        val expectedText = "If you need more medication sooner, contact your GP."

        Assert.assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)

        Assert.assertEquals("expected error text $expectedSubHeader but found ${errorPage.subHeading.text}",
                expectedSubHeader, errorPage.subHeading.text)

        Assert.assertEquals("expected error text $expectedText but found ${errorPage.errorText1.text}",
                expectedText, errorPage.errorText1.text)
    }
}
