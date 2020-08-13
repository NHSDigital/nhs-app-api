package features.myrecord.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.myrecord.factories.ProceduresFactoryVision
import utils.SerenityHelpers

class MedicalRecordProceduresStepDefinitionsBackend {

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

    @Then("^an error occurred retrieving the procedures")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.errorRetrieving(SerenityHelpers.getPatient())
    }
}
