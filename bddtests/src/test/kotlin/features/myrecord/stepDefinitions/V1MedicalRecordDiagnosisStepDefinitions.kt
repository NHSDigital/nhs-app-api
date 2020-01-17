package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DiagnosisFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers

open class V1MedicalRecordDiagnosisStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page
    private lateinit var diagnosisFactoryVision: DiagnosisFactoryVision

    @Given( "^I do not have access to diagnosis$" )
    fun givenIDoNotHaveAccessToDiagnosis(){
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.noAccess(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple diagnosis$")
    fun andTheGpPracticeHasMultipleDiagnosis(){
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^an error occurred retrieving the diagnosis")
    fun andAnErrorOccurredRetrievingTheDiagnosis() {
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("^the GP practice responds with bad diagnosis data")
    fun theGpPracticeRespondsWithBadDiagnosisData(){
        DiagnosisFactoryVision().badData(SerenityHelpers.getPatient())
    }

    @When("^I click the diagnosis section$" )
    fun whenIClickTheDiagnosisSection() {
        medicalRecordV1Page.diagnosis.toggleShrub()
    }

    @When("^I enter url address for diagnosis detail directly into the url$")
    fun whenIEnterUrlAddressForDiagnosisDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/diagnosis-detail"
        browser.browseTo(fullUrl)
    }

    @Then( "^I see diagnosis information - Medical Record v1$" )
    fun thenISeeDiagnosisInformationV1() {
        val sectionName = "Diagnosis"
        Assert.assertTrue(medicalRecordV1Page.isVisionSectionPageVisible(sectionName, sectionName))
    }
}
