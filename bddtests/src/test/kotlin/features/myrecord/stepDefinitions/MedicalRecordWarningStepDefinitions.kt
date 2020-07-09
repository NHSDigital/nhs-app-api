package features.myrecord.stepDefinitions

import cucumber.api.DataTable
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.myrecord.MyRecordWarningPage
import pages.navigation.NavBarNative
import utils.toSingleElementList

open class MedicalRecordWarningStepDefinitions {

    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var browser: BrowserSteps

    private lateinit var myRecordWarningPage: MyRecordWarningPage

    @Given("^I navigate to the Medical Record Page$")
    fun iNavigateToTheMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @Then("^the Medical Record Warning Page is displayed$")
    fun theMedicalRecordWarningPageIsDisplayed() {
        myRecordWarningPage.assertContent()
    }

    @Then("^retrieving the Medical Record pages directly displays the Medical Record Warning page$")
    fun retrievingTheMedicalRecordPageDirectlyDisplaysTheMedicalRecordWarningPage(table: DataTable) {
        table.toSingleElementList().forEach {url ->
            browser.browseToInternal(url)
            myRecordWarningPage.assertContent()
        }
    }
}
