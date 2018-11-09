package features.prescriptions.steps

import com.google.gson.GsonBuilder
import models.prescriptions.HistoricPrescription
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.prescription.PrescriptionsPage
import pages.prescription.RepeatPrescriptionsPage

open class PrescriptionsSteps {

    lateinit var prescriptions : PrescriptionsPage
    private lateinit var repeatPrescriptions: RepeatPrescriptionsPage
    private lateinit var errorPage: ErrorPage

    @Step
    open fun isLoaded() {
        prescriptions.isLoaded()
    }

    @Step
    fun assertPrescriptionsMatch(list : List<HistoricPrescription>,
                                 expectedPrescriptions : Int,
                                 providerHasAllPrescriptionFields: Boolean = true) {

        val gson = GsonBuilder().setPrettyPrinting().create()
        val p = prescriptions.getAllPrescriptions(providerHasAllPrescriptionFields)

        val actualJson = gson.toJson(p).toString()
        val expectedJson = gson.toJson(list).toString()

        assertEquals(expectedJson, actualJson)
        assertEquals(expectedPrescriptions, p.count())
    }

    @Step
    fun assertNoRepeatPrescriptionsMessageShown() {
        assertTrue(prescriptions.isNoPrescriptionsMessageVisible())
    }

    @Step
    fun selectSubscriptionsToOrder(howMany: Int){
        repeatPrescriptions.selectXPrescriptionsToOrder(howMany)
    }

    @Step
    fun clickContinue() {
        repeatPrescriptions.orderRepeatPrescriptionButton.click()
    }

    @Step
    fun clickConfirmAndOrderRepeat() {
        repeatPrescriptions.clickConfirmAndOrderRepeatSubscriptionButton()
    }
}
