package features.oneOneOneOnline.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.CheckMySymptomsPage


open class CheckMySymptoms {

    lateinit var checkMySymptoms: CheckMySymptomsPage


    @Step
    fun assertConditionsHeaderVisible() {
        Assert.assertTrue("Conditions header not visible, expected to be visible",
                checkMySymptoms.isConditionsHeaderVisible())
    }

    @Step
    fun assertNhs111HeaderVisible() {
        Assert.assertTrue("Conditions header not visible, expected to be visible",
                checkMySymptoms.isNhs111HeaderVisible())
    }

    @Step
    fun assertCoronaHeaderVisible() {
        Assert.assertTrue("Corona header not visible, expected to be visible",
                checkMySymptoms.isCoronaHeaderVisible())
    }

    @Step
    fun clickConditionsHeader() {
        checkMySymptoms.clickConditionsHeader()
    }

    @Step
    fun clickNHS111Header() {
        checkMySymptoms.clickNHS111Header()
    }

    @Step
    fun clickCoronaVirusHeader() {
        checkMySymptoms.clickCoronaVirusHeader()
    }
}
