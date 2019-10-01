package features.gpMedicalRecord.stepDefinitions

import mocking.MockingClient
import models.Patient
import utils.SerenityHelpers

const val HTTP_EXCEPTION = "HttpException"

abstract class AbstractDemographicsStepDefinitions {
    val mockingClient = MockingClient.instance

    fun setPatientToDefaultFor(gpSystem: String) {
        SerenityHelpers.setPatient(Patient.getDefault(gpSystem))
        SerenityHelpers.setGpSupplier(gpSystem)
    }
}
