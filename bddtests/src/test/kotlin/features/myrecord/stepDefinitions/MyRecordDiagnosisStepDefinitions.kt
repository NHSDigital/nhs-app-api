package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DiagnosisFactoryVision
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage

open class MyRecordDiagnosisStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var diagnosisFactoryVision: DiagnosisFactoryVision


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
        Assert.assertTrue(myRecordInfoPage.isDiagnosisPageVisible())
    }

    @And("^an error occurred retrieving the diagnosis")
    fun andAnErrorOccurredRetrievingTheDiagnosisFor() {
        setPatientToDefaultFor("VISION")
        diagnosisFactoryVision = DiagnosisFactoryVision()
        diagnosisFactoryVision.errorRetrieving(patient)
    }

    private fun assertTextInSection(heading:String, message: String){
        val section = myRecordInfoPage.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }
}
