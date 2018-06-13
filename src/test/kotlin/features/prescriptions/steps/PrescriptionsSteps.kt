package features.prescriptions.steps

import com.google.gson.GsonBuilder
import models.prescriptions.HistoricPrescription
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.prescription.PrescriptionsPage
import pages.prescription.RepeatPrescriptionsPage

open class PrescriptionsSteps {

    lateinit var prescriptions : PrescriptionsPage
    lateinit var repeatPrescriptions: RepeatPrescriptionsPage

    @Step
    open fun isLoaded() {
        Assert.assertTrue(prescriptions.isLoaded())
    }

    @Step
    fun assertPrescriptionsMatch(list : ArrayList<HistoricPrescription>, expectedPrescriptions : Int) {

        val gson = GsonBuilder().setPrettyPrinting().create()
        var p = prescriptions.getAllPrescriptions()

        Assert.assertTrue(gson.toJson(p).toString().equals(gson.toJson(list).toString()))
        Assert.assertTrue(p.count() == expectedPrescriptions)
    }

    @Step
    fun assertNoRepeatPrescriptionsMessageShown() {
        Assert.assertTrue(prescriptions.isNoPrescriptionsMessageVisible())
    }

    @Step
    fun assertCorrectErrorMessageShown(pageTitle: String,
                                       pageHeaderText: String,
                                       headerText: String,
                                       subHeaderText: String,
                                       messageText: String,
                                       retryButtonText: String){
        Assert.assertTrue(prescriptions.isErrorMessageContentCorrect(pageTitle, pageHeaderText, headerText, subHeaderText, messageText, retryButtonText))
    }

    @Step
    fun selectSubscriptionsToOrder(howMany: Int){
        repeatPrescriptions.selectXPrescriptionsToOrder(howMany)
    }

    @Step
    fun clickContinue() {
        repeatPrescriptions.clickContinueButton()
    }

    @Step
    fun clickConfirmAndOrderRepeat() {
        repeatPrescriptions.clickConfirmAndOrderRepeatSubscriptionButton()
    }
}
