package features.authentication.steps

import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
import pages.navigation.HeaderNative
import pages.navigation.WebHeader

enum class PatientDetail(val label: String) {
    NHS_NUMBER("NHS number"),
    DOB("Date of birth");

    companion object {
        fun fromLabel(label: String): PatientDetail = values().first { it.label == label }
    }
}

open class HomeSteps {

    lateinit var homePage: HomePage

    lateinit var headerNative: HeaderNative

    @Steps
    lateinit var webHeader: WebHeader

    @Step
    fun assertWelcomeMessageShownFor(patient: Patient) {
        homePage.assertHasWelcomeMessageFor(patient)
    }

    @Step
    fun assertPatientDetailIsVisible(patient: Patient, detail: PatientDetail) {
        homePage.assertPatientDetailIsVisible(detail.label, getDetailValue(patient, detail))
    }

    @Step
    fun assertPatientDetailIsNotPresent(detail: PatientDetail) {
        homePage.assertPatientDetailIsNotPresent(detail.label)
    }

    @Step
    fun assertProxyPatientDetailsShownFor(proxyProfile: LinkedProfileFacade) {
        homePage.assertHasProxyPatientDetails(proxyProfile, getExpectedProxyDetails(proxyProfile))
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

    private fun getExpectedProxyDetails(proxyPatient: LinkedProfileFacade): ArrayList<String> {
        return arrayListOf(
                "Age: ${proxyPatient.profile.formattedAge()}",
                "GP surgery: ${proxyPatient.gpPracticeName}"
        )
    }

    private fun getDetailValue(patient: Patient, detail: PatientDetail): String {
        return when (detail) {
            PatientDetail.DOB -> patient.formattedDateOfBirth()
            PatientDetail.NHS_NUMBER -> patient.formattedNHSNumber()
        }
    }
}
