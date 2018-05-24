package features.prescriptions.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.PrescriptionsPage

open class PrescriptionsSteps {

    lateinit var prescriptions: PrescriptionsPage

    @Step
    open fun isLoaded() {
        Assert.assertTrue(prescriptions.isLoaded())
    }

    fun assertNoRepeatPrescriptionsMessageShown() {
        Assert.assertTrue(prescriptions.isNoPrescriptionsMessageVisible())
    }
}