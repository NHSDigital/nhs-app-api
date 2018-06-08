package features.prescriptions.steps

import com.google.gson.GsonBuilder
import models.prescriptions.HistoricPrescription
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.PrescriptionsPage

open class PrescriptionsSteps {

    lateinit var prescriptions: PrescriptionsPage

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
}