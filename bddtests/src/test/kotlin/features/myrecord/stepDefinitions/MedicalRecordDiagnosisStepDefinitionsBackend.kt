package features.myrecord.stepDefinitions

import io.cucumber.java.en.Given
import features.myrecord.factories.DiagnosisFactoryVision
import utils.SerenityHelpers

open class MedicalRecordDiagnosisStepDefinitionsBackend {

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
}
