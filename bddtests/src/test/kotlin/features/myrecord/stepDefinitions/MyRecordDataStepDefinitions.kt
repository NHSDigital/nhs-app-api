package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import features.myrecord.factories.MyRecordFactory
import utils.SerenityHelpers

class MyRecordDataStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled all medical records for the patient$")
    fun givenTheGPPracticeHasEnabledAllMedicalRecordsForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = MyRecordStepDefinitions().setSupplierAndPatientForV1MedicalRecord(gpSystem.supplierName)

        MyRecordFactory.getForSupplier(gpSystem).enabledWithAllRecords(patient)
    }
}