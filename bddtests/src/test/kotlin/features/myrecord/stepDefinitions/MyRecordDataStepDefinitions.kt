package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import features.myrecord.factories.MyRecordFactory
import utils.SerenityHelpers

class MyRecordDataStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled all medical records for the patient$")
    fun givenTheGPPracticeHasEnabledAllMedicalRecordsForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        MyRecordFactory.getForSupplier(gpSystem).enabledWithAllRecords(SerenityHelpers.getPatient())
    }
}