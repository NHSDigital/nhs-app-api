package features.authentication.steps

import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.HomePage
import pages.HybridPageElement
import pages.assertIsVisible
import pages.navigation.WebHeader

enum class PatientDetail(val label: String) {
    NAME("Name"),
    NHS_NUMBER("NHS number"),
    DOB("Date of birth");

    companion object {
        fun fromLabel(label: String): PatientDetail = values().first { it.label == label }
    }
}

enum class NavigationLinkText (val linkText: String) {
    GP_HEALTH_RECORD("View your GP health record"),
    PRESCRIPTIONS("Order a repeat prescription"),
    MESSAGES("View your messages"),
    UNREAD_MESSAGES("View your messages"),
    LINKED_PROFILES("Linked profiles"),
}

open class HomeSteps {

    lateinit var homePage: HomePage

    @Steps
    lateinit var webHeader: WebHeader

    fun assertPatientNameIsVisible(patient: Patient) {
        val name = patient.formattedFullName(false).toUpperCase()
        homePage.assertPatientNameIsVisible(name)
    }

    fun assertNhsNumberIsVisible(patient: Patient) {
        val number = patient.formattedNHSNumber()
        homePage.assertNhsNumberIsVisible(number)
    }

    @Step
    fun assertPatientNhsNumberIsNotPresent() {
        homePage.assertNhsNumberIsNotPresent()
    }

    @Step
    fun assertProxyPatientDetailsShownFor(proxyProfile: LinkedProfileFacade) {
        homePage.assertHasProxyPatientDetails(getExpectedProxyDetails(proxyProfile))
    }

    @Step
    fun assertHeaderVisible() {
        webHeader.getPageTitle().withText("Access your NHS services any time, day or night")
        homePage.userInfoDisplay.assertIsVisible()
    }

    @Step
    fun assertProxyHeaderVisible() {
        webHeader.getPageTitle().withText("Access your NHS services any time, day or night")
        homePage.userInfoDisplayProxy.assertIsVisible()
    }

    @Step
    fun waitForLoginToCompleteSuccessfully(waitForLoginPage: Boolean) {
        if (waitForLoginPage) {
            assertHeaderVisible()
        }
    }

    @Step
    fun assertLinkIsVisible(link: NavigationLinkText): HybridPageElement {
        return homePage.assertLinkIsVisible(link.linkText)
    }

    fun assertUnreadCountIndicatorIsDisplayed() {
        homePage.assertUnreadCountPresent()
    }

    fun assertUnreadMessageIndicatorIsNotDisplayed() {
        homePage.assertUnreadIndicatorNotPresent()
    }

    fun assertUnreadCountIndicatorIsNotDisplayed() {
        homePage.assertUnreadCountNotPresent()
    }

    private fun getExpectedProxyDetails(proxyPatient: LinkedProfileFacade): ArrayList<String> {
        return arrayListOf(
            "${proxyPatient.profile.formattedFullName(true).toUpperCase()}",
            "${proxyPatient.profile.age.formattedAge()}",
            "${proxyPatient.gpPracticeName}"
        )
    }
}
