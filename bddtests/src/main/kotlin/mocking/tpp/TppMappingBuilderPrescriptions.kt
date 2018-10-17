package mocking.tpp

import mocking.tpp.prescriptions.TppPrescriptionsBuilder
import mocking.tpp.prescriptionsSubmission.TppPrescriptionsSubmissionBuilder
import models.Patient

class TppMappingBuilderPrescriptions{

    fun listRepeatMedication(patient: Patient) = TppPrescriptionsBuilder(patient)

    fun prescriptionSubmission(patient: Patient, drugIds: List<String>?) =
            TppPrescriptionsSubmissionBuilder(patient, drugIds)
}