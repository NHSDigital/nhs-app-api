package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProceduresFactoryVision
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage

class MyRecordProceduresStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var  proceduresFactoryVision: ProceduresFactoryVision

    @Then("^I see the procedures heading$" )
    fun thenISeeTheProceduresHeading() {
        val header = "Procedures"
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @And( "^I do not have access to procedures$" )
    fun givenIDoNotHaveAccessToProceduresFor(){
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.noAccess(patient)
    }

    @And("^the GP Practice has multiple procedures$")
    fun andTheGpPracticeHasMultipleProceduresFor(){
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.enabledWithRecords(patient)
    }

    @When("^I click the procedures section$" )
    fun whenIClickTheProceduresSection() {
        myRecordInfoPage.procedures.toggleShrub()
    }

    @And("procedures$" )
    fun andIHaveNoProcedures() {
        assertTextInSection("Procedures", "No information recorded")
    }

    @And("^an error occurred retrieving the procedures")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.errorRetrieving(patient)
    }

    private fun assertTextInSection(heading:String, message: String){
        val section = myRecordInfoPage.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }

}