package features.authentication.steps

import models.Patient
import net.thucydides.core.annotations.Step
import org.joda.time.DateTime
import org.junit.Assert
import pages.HomePage
import pages.navigation.Header

open class HomeSteps {

    lateinit var homePage: HomePage
    lateinit var header: Header

    @Step
    fun assertPageIsVisible() {
        assertHeaderVisible()
    }

    @Step
    fun assertWelcomeMessageShownFor(patient: Patient) {
        homePage.assertHasWelcomeMessageFor(patient)
    }

    fun getExpectedDetails(patient:Patient): ArrayList<String> {
        return arrayListOf(
                "Date of birth: ${formatDOB(patient.dateOfBirth)}",
                "NHS number: ${formatNHSNumber(patient.nhsNumbers.first())}")
    }

    private fun formatDOB(dob: String):String{
        return DateTime.parse(dob).toString("d MMM yyyy")
    }

    private fun formatNHSNumber(number : String):String{
        return "${number.substring(0, 3)} ${number.substring(3, 6)} ${number.substring(6, 10)}"
    }

    @Step
    fun assertPatientDetailsShownFor(patient:Patient ) {
        homePage.assertHasPatientDetails(patient,getExpectedDetails(patient))
    }

    @Step
    fun assertHeaderVisible() {
        Assert.assertTrue(header.isVisible(homePage.headerText))
    }

    @Step
    fun waitForLoginToComplete() {
        homePage.waitForSpinnerToDisappear()
    }
}