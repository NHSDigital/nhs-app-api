package features.authentication.steps

import models.Patient
import net.thucydides.core.annotations.Step
import org.joda.time.DateTime
import org.junit.Assert
import pages.HomePage
import pages.navigation.HeaderNative

open class HomeSteps {

    lateinit var homePage: HomePage

    lateinit var headerNative: HeaderNative

    @Step
    fun assertWelcomeMessageShownFor(patient: Patient) {
        homePage.assertHasWelcomeMessageFor(patient)
    }

    fun getExpectedDetails(patient:Patient): ArrayList<String> {
        return arrayListOf(
                "Date of birth: ${patient.formattedDateOfBirth()}",
                "NHS number: ${patient.formattedNHSNumber()}")
    }

    @Step
    fun assertPatientDetailsShownFor(patient:Patient ) {
        homePage.assertHasPatientDetails(patient,getExpectedDetails(patient))
    }

    @Step
    fun assertHeaderVisible() {
        headerNative.waitForPageHeaderText("Home")
        Assert.assertTrue(homePage.isWelcomeHeaderVisible())
    }

    @Step
    fun waitForLoginToComplete() {
        homePage.waitForSpinnerToDisappear()
    }
}
