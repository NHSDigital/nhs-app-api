package features.authentication.steps

import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
import pages.navigation.HeaderNative
import pages.navigation.WebHeader

open class HomeSteps {

    lateinit var homePage: HomePage

    lateinit var headerNative: HeaderNative

    @Steps
    lateinit var webHeader: WebHeader

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
        webHeader.getPageTitle().withText("Home")
        Assert.assertTrue(homePage.isWelcomeHeaderVisible())
    }

    @Step
    fun assertLinkedProfileLinkVisible() {
        headerNative.waitForPageHeaderText("Home")
        Assert.assertTrue(homePage.isLinkedProfileVisible())
    }

    @Step
    fun waitForLoginToCompleteSuccessfully() {
        homePage.locatorMethods.assertNativeElementsLoaded(homePage.greeting)
        if (homePage.onMobile()) {
            Assert.assertEquals("Dismiss button text", "Dismiss", homePage.dismissButton.textValue.trim())
            homePage.dismissButton.click()
        }
    }
}
