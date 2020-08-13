package features.myrecord.stepDefinitions

import io.cucumber.java.en.Given
import features.myrecord.factories.ExaminationsFactoryVision
import utils.SerenityHelpers

open class MedicalRecordExaminationsStepDefinitionsBackend {

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
}
