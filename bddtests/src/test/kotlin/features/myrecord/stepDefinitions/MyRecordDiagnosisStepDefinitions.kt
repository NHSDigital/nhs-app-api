package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DiagnosisFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage

open class MyRecordDiagnosisStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var diagnosisFactoryVision: DiagnosisFactoryVision
    @Steps
    lateinit var browser: BrowserSteps


    @Then("^I see the diagnosis heading$" )
    fun thenISeeTheDiagnosisHeading() {
        val header = "Diagnosis"
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @And("^I see the diagnosis section collapsed$" )
    fun andISeeTheDiagnosisSectionCollapsed(){
        Assert.assertFalse(myRecordInfoPage.isDiagnosisTextMsgVisible())
    }

    @And( "^I do not have access to diagnosis$" )
    fun givenIDoNotHaveAccessToDiagnosisFor(){
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.noAccess(patient)
    }

    @And("^the GP Practice has multiple diagnosis$")
    fun andTheGpPracticeHasMultipleDiagnosisFor(){
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.enabledWithRecords(patient)
    }

    @When("^I click the diagnosis section$" )
    fun whenIClickTheDiagnosisSection() {
        myRecordInfoPage.diagnosis.toggleShrub()
    }

    @Then( "^I see diagnosis information$" )
    fun thenISeeDiagnosisInformation() {
        val sectionName = "Diagnosis"
        Assert.assertTrue(myRecordInfoPage.isVisionSectionPageVisible(sectionName, sectionName))
    }

    @And("^an error occurred retrieving the diagnosis")
    fun andAnErrorOccurredRetrievingTheDiagnosisFor() {
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.errorRetrieving(patient)
    }

    @When("^I enter url address for diagnosis detail directly into the url$")
    fun whenIEnterUrlAddressForDiagnosisDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/diagnosis-detail"
        browser.browseTo(fullUrl)
    }
}
