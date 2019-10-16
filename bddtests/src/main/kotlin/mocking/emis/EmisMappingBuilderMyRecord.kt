package mocking.emis

import mocking.emis.allergies.EmisAllergiesBuilder
import mocking.emis.consultations.EmisConsultationsBuilder
import mocking.emis.demographics.EmisDemographicsBuilder
import mocking.emis.documents.EmisDocumentBuilder
import mocking.emis.documents.EmisDocumentsBuilder
import mocking.emis.immunisations.EmisImmunisationsBuilder
import mocking.emis.medications.EmisMedicationsBuilder
import mocking.emis.problems.EmisProblemsBuilder
import mocking.emis.testResults.EmisTestResultsBuilder
import models.Patient

class EmisMappingBuilderMyRecord(private var configuration: EmisConfiguration?){
    fun demographicsRequest(patient: Patient) = EmisDemographicsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId, patient.sessionId)

    fun allergiesRequest(patient: Patient) = EmisAllergiesBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun medicationsRequest(patient: Patient) = EmisMedicationsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun problemsRequest(patient: Patient) = EmisProblemsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun consultationsRequest(patient: Patient) = EmisConsultationsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun immunisationsRequest(patient: Patient) = EmisImmunisationsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun testResultsRequest(patient: Patient) = EmisTestResultsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun documentsRequest(patient: Patient) = EmisDocumentsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun documentRequest(patient: Patient, documentId: String) = EmisDocumentBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId,
            documentId)
}