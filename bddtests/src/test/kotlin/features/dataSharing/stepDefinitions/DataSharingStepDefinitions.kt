package features.dataSharing.stepDefinitions

import config.Config
import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import org.junit.Assert
import pages.ndop.ChoiceApplyDataSharingPage
import pages.ndop.ConfidentialDataSharingPage
import pages.ndop.DataSharingPage
import pages.ndop.MakeChoiceDataSharingPage
import pages.ndop.NdopPage
import pages.ndop.OverviewDataSharingPage
import utils.SerenityHelpers
import java.net.URL

class DataSharingStepDefinitions {

    private lateinit var overviewPage: OverviewDataSharingPage
    private lateinit var confidentialDataSharingPage: ConfidentialDataSharingPage
    private lateinit var choiceApplyDataSharingPage: ChoiceApplyDataSharingPage
    private lateinit var makeChoiceDataSharingPage: MakeChoiceDataSharingPage
    private lateinit var ndopPage: NdopPage

    private val mockingClient = MockingClient.instance

    @Given("^I am a user who wishes to manage their Data Sharing Preferences$")
    fun iAmAUserWhoWishesToManageTheirDataSharingPreferences() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        mockingClient.forNdop.mock { postTokenToNdop().respondWithNdopMockPage() }
    }

    @When("^I click the '(.*)' contents link on the Data Sharing page$")
    fun iClickTheXContentsLink(link: String) {
        overviewPage.contentsLink(link).click()
    }

    @When("^I click the Previous button on the Data Sharing page$")
    fun iClickThePreviousButton() {
        overviewPage.previousButton().click()
    }

    @When("^I click the Next button on the Data Sharing page$")
    fun iClickTheNextButton() {
        overviewPage.nextButton().click()
    }

    @When("^I click the Start Now button on the Data Sharing page$")
    fun iClickTheStartNowButton() {
        makeChoiceDataSharingPage.btnStartNow.click()
    }

    @Then("^the Data Sharing '(.*)' page is displayed$")
    fun theDataSharingPageIsDisplayed(page: String) {
        getPage(page).assertDisplayed()
    }

    @Then("^the content on the Data Sharing '(.*)' page is correct$")
    fun theContentOnTheDataSharingPageIsCorrect(page: String) {
        getPage(page).assertContent()
    }

    @Then("^the NDOP website is displayed$")
    fun iAmOnTheNDOPWebsite() {
        Assert.assertTrue("Expected token to be displayed", ndopTokenIsDisplayed())
    }

    private fun ndopTokenIsDisplayed(): Boolean {
        if(ndopPage.onMobile()){
            URL(Config.instance.dataPreferencesUrl)
        }
        return ndopPage.tokenIsDisplayed()
    }

    private fun getPage(title: String): DataSharingPage {
        return when (title) {
            "Overview" -> overviewPage
            "How confidential patient information is used" -> confidentialDataSharingPage
            "When your choice does not apply" -> choiceApplyDataSharingPage
            "Make your choice" -> makeChoiceDataSharingPage
             else -> throw AssertionError("$title is not a valid page name.")
        }
    }
}
