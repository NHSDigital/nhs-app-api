package features.prescriptions.steps

import com.google.gson.GsonBuilder
import models.prescriptions.HistoricPrescription
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
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
    fun assertPrescriptionsMatch(list : List<HistoricPrescription>, expectedPrescriptions : Int, isEmis: Boolean = true) {

        val gson = GsonBuilder().setPrettyPrinting().create()
        var p = prescriptions.getAllPrescriptions(isEmis)

        var expectedJson = gson.toJson(p).toString()
        var actualJson = gson.toJson(list).toString()

        Assert.assertTrue(expectedJson.equals(actualJson))
        assertEquals(expectedPrescriptions, p.count())
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
        Assert.assertTrue("Expected error message: { " +
                "page title: $pageTitle, " +
                "page header text: $pageHeaderText, " +
                "header text: $headerText, " +
                "sub-header text: $subHeaderText, " +
                "message text: $messageText, " +
                "retry button text: $retryButtonText } ",
                prescriptions.isErrorMessageContentCorrect(pageTitle, pageHeaderText, headerText, subHeaderText, messageText, retryButtonText))
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
