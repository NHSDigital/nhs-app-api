package mocking.emis

import mocking.emis.allergies.EmisAllergiesBuilder
import mocking.emis.consultations.EmisConsultationsBuilder
import mocking.emis.documents.EmisDocumentsBuilder
import mocking.emis.immunisations.EmisImmunisationsBuilder
import mocking.emis.medications.EmisMedicationsBuilder
import mocking.emis.problems.EmisProblemsBuilder
import mocking.emis.testResults.EmisTestResultsBuilder
import models.Patient

class EmisProxyMappingBuilderMyRecord(private var configuration: EmisConfiguration?){

    fun allergiesRequestAsProxy(patient: Patient, actingOnBehalfOf: Patient) = EmisAllergiesBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun medicationsRequest(patient: Patient,
                           actingOnBehalfOf: Patient) = EmisMedicationsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun problemsRequestAsProxy(patient: Patient, actingOnBehalfOf: Patient) = EmisProblemsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun consultationsRequestAsProxy(patient: Patient,
                                    actingOnBehalfOf: Patient) = EmisConsultationsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun immunisationsRequestAsProxy(patient: Patient,
                                    actingOnBehalfOf: Patient) = EmisImmunisationsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun testResultsRequestAsProxy(patient: Patient, actingOnBehalfOf: Patient) = EmisTestResultsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun documentsRequestAsProxy(patient: Patient,  actingOnBehalfOf: Patient) = EmisDocumentsBuilder(configuration!!,
            actingOnBehalfOf.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)
}
