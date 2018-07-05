package features.myrecord.stepDefinitions

import mocking.MockingClient
import models.Patient

const val HTTP_EXCEPTION = "HttpException"

abstract class AbstractDemographicsStepDefinitions {
    val mockingClient = MockingClient.instance
    lateinit var patient: Patient

    fun setPatientToDefaultFor(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
    }
}