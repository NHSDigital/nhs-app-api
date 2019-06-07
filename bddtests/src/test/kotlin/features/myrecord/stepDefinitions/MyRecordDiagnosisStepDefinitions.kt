package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DiagnosisFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers

open class MyRecordDiagnosisStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var diagnosisFactoryVision: DiagnosisFactoryVision

    @Given( "^I do not have access to diagnosis$" )
    fun givenIDoNotHaveAccessToDiagnosisFor(){
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.noAccess(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple diagnosis$")
    fun andTheGpPracticeHasMultipleDiagnosisFor(){
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^an error occurred retrieving the diagnosis")
    fun andAnErrorOccurredRetrievingTheDiagnosisFor() {
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.errorRetrieving(SerenityHelpers.getPatient())
    }

    @When("^I click the diagnosis section$" )
    fun whenIClickTheDiagnosisSection() {
        myRecordInfoPage.diagnosis.toggleShrub()
    }

    @When("^I enter url address for diagnosis detail directly into the url$")
    fun whenIEnterUrlAddressForDiagnosisDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/diagnosis-detail"
        browser.browseTo(fullUrl)
    }

    @Then( "^I see diagnosis information$" )
    fun thenISeeDiagnosisInformation() {
        val sectionName = "Diagnosis"
        Assert.assertTrue(myRecordInfoPage.isVisionSectionPageVisible(sectionName, sectionName))
    }
}
