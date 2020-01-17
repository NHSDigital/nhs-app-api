package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ExaminationsFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers

open class V1MedicalRecordExaminationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page
    private lateinit var examinationsFactoryVision: ExaminationsFactoryVision

    @Given("^I do not have access to examinations$")
    fun givenIDoNotHaveAccessToExaminations() {
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.noAccess(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple examinations$")
    fun andTheGpPracticeHasMultipleExaminations() {
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^an error occurred retrieving the examinations")
    fun andAnErrorOccurredRetrievingTheExaminations() {
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("the GP practice responds with bad examinations data")
    fun gpPracticeRespondsWithBadExaminationsData() {
        ExaminationsFactoryVision().badData(SerenityHelpers.getPatient())
    }

    @When("^I enter url address for examinations detail directly into the url$")
    fun whenIEnterUrlAddressForExaminationsDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/examinations-detail"
        browser.browseTo(fullUrl)
    }

    @Then( "^I see examinations information - Medical Record v1$" )
    fun thenISeeExaminationsInformationV1() {
        val sectionName = "Examination"
        Assert.assertTrue(medicalRecordV1Page.isVisionSectionPageVisible(sectionName, sectionName + "s"))
    }
}
