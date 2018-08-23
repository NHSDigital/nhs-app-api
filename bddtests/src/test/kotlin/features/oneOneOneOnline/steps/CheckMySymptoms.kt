package features.oneOneOneOnline.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.CheckMySymtomsPage


open class CheckMySymptoms {

    lateinit var checkMySymptoms: CheckMySymtomsPage


    @Step
    fun assertConditionsHeaderVisible() {
        Assert.assertTrue("Conditions header not visible, expected to be visible", checkMySymptoms.isConditionsHeaderVisible())
    }

    @Step
    fun assertNhs111HeaderVisible() {
        Assert.assertTrue("Conditions header not visible, expected to be visible", checkMySymptoms.isNhs111HeaderVisible())
    }

    @Step
    fun waitForSpinnerToDisappear() {
        checkMySymptoms.waitForSpinnerToDisappear()
    }
}
