package features.authentication.steps

import mockingFacade.linkedProfiles.LinkedProfileFacade
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

    fun getExpectedProxyDetails(proxyPatient:LinkedProfileFacade): ArrayList<String> {
        return arrayListOf(
                "Age: ${proxyPatient.profile.formattedAge()}",
                "GP surgery: ${proxyPatient.gpPracticeName}"
        )
    }

    @Step
    fun assertPatientDetailsShownFor(patient:Patient ) {
        homePage.assertHasPatientDetails(patient,getExpectedDetails(patient))
    }

    @Step
    fun assertProxyPatientDetailsShownFor(proxyProfile:LinkedProfileFacade ) {
        homePage.assertHasProxyPatientDetails(proxyProfile,getExpectedProxyDetails(proxyProfile))
    }


    @Step
    fun assertHeaderVisible() {
        webHeader.getPageTitle().withText("Home")
        Assert.assertTrue(homePage.isWelcomeHeaderVisible())
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
