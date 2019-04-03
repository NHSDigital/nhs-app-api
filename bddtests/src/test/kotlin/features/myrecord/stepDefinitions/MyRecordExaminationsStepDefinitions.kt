package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ExaminationsFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage

open class MyRecordExaminationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var examinationsFactoryVision: ExaminationsFactoryVision

    @Given("^I do not have access to examinations$")
    fun givenIDoNotHaveAccessToExaminations() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.noAccess(patient)
    }

    @Given("^the GP Practice has multiple examinations$")
    fun andTheGpPracticeHasMultipleExaminationsFor() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.enabledWithRecords(patient)
    }

    @Given("^an error occurred retrieving the examinations")
    fun andAnErrorOccurredRetrievingTheExaminationsFor() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.errorRetrieving(patient)
    }

    @When("^I click the examinations section$")
    fun whenIClickTheExaminationsSection() {
        myRecordInfoPage.examinations.toggleShrub()
    }

    @When("^I enter url address for examinations detail directly into the url$")
    fun whenIEnterUrlAddressForExaminationsDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/examinations-detail"
        browser.browseTo(fullUrl)
    }

    @Then( "^I see examinations information$" )
    fun thenISeeExaminationsInformation() {
        val sectionName = "Examination"
        Assert.assertTrue(myRecordInfoPage.isVisionSectionPageVisible(sectionName, sectionName + "s"))
    }
}
