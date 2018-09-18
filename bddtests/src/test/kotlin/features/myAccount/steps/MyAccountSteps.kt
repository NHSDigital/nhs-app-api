package features.myAccount.steps

import features.sharedStepDefinitions.SharedStepDefinitions
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.MyAccountPage
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.time.LocalDate
import java.time.format.TextStyle
import java.util.*

open class MyAccountSteps {

    lateinit var myAccountPage: MyAccountPage

    @Step
    fun signOut() {
        myAccountPage.signOutButton.element.click()
    }

    @Step
    fun assertSignoutButtonVisible() {
        Assert.assertTrue("Sign out button not visible, expected to be visible", myAccountPage.isSignOutButtonVisible())
    }

    @Step
    fun assertAboutUsHeaderVisible() {
        Assert.assertTrue("About us header not visible, expected to be visible", myAccountPage.isAboutUsHeaderVisible())
    }

    @Step
    fun goToTermsOfUse() {
        myAccountPage.clickTermsOfUseLink()
    }

    @Step
    fun goToPrivacyPolicy() {
        myAccountPage.clickPrivacyPolicyLink()
    }

    @Step
    fun goToCookiesPolicy() {
        myAccountPage.clickCookiesPolicyLink()
    }

    @Step
    fun goToOpenSourceLicenses() {
        myAccountPage.clickOpenSourceLicensesLink()
    }

    @Step
    fun goToHelpAndSupport() {
        myAccountPage.clickHelpAndSupportLink()
    }

    @Step
    fun checkAccountDetailsShowing() {
        var rawDob = SharedStepDefinitions.patient.dateOfBirth
        var parsedDob = LocalDate.parse(rawDob)

        var name = SharedStepDefinitions.patient.title + " " + SharedStepDefinitions.patient.firstName + " " + SharedStepDefinitions.patient.surname
        var dob = parsedDob.dayOfMonth.toString() + " " + parsedDob.month.getDisplayName(TextStyle.SHORT, Locale.ENGLISH) + " " + parsedDob.year.toString()
        var nhsNumber = SharedStepDefinitions.patient.nhsNumbers[0]

        Assert.assertTrue(myAccountPage.arePersonalDetailsVisible(name, dateOfBirth = dob, nhsNumber = nhsNumber))
    }
}
