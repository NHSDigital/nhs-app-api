package mocking.microtest

import mocking.microtest.demographics.DemographicsBuilderMicrotest
import models.Patient

class MicrotestMappingBuilderDemographics {

    fun demographicsRequest(patient: Patient): DemographicsBuilderMicrotest {
        return DemographicsBuilderMicrotest(patient)
    }
}