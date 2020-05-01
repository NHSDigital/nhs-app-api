package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProceduresFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers

class V1MedicalRecordProceduresStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page
    private lateinit var proceduresFactoryVision: ProceduresFactoryVision

    @Given( "^I do not have access to procedures$" )
    fun givenIDoNotHaveAccessToProcedures(){
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.noAccess(SerenityHelpers.getPatient())
    }

    @When("^the GP Practice has multiple procedures$")
    fun andTheGpPracticeHasMultipleProcedures(){
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.enabledWithRecords(SerenityHelpers.getPatient())
    }

    @When("^the GP Practice responds with bad procedures data")
    fun theGpPracticeRespondsWithBadData(){
        ProceduresFactoryVision().badData(SerenityHelpers.getPatient())
    }

    @When("^I click the procedures section$" )
    fun whenIClickTheProceduresSection() {
        medicalRecordV1Page.procedures.toggleShrub()
    }

    @Then("^an error occurred retrieving the procedures")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.errorRetrieving(SerenityHelpers.getPatient())
    }

    @Then( "^I see procedures information - Medical Record v1$" )
    fun thenISeeProceduresInformationV1() {
        val sectionName = "Procedures"
        Assert.assertTrue(medicalRecordV1Page.isVisionSectionPageVisible(sectionName, sectionName))
    }
}
