package mocking.microtest

import mocking.microtest.demographics.DemographicsBuilderMicrotest
import mocking.microtest.myRecord.MyRecordBuilderMicrotest

import models.Patient

class MicrotestMappingBuilderMyRecord {

    fun demographicsRequest(patient: Patient) = DemographicsBuilderMicrotest(patient)

    fun myRecordRequest(patient: Patient) = MyRecordBuilderMicrotest(patient.odsCode, patient.nhsNumbers.first())

}
