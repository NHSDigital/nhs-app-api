package mocking.stubs.myMedicalRecord

import mocking.MockingClient
import mocking.stubs.myMedicalRecord.emis.AllergiesStubs
import mocking.stubs.myMedicalRecord.emis.ConsultationsStubs
import mocking.stubs.myMedicalRecord.emis.DemographicsStubs
import mocking.stubs.myMedicalRecord.emis.ImmunisationsStubs
import mocking.stubs.myMedicalRecord.emis.MedicationsStubs
import mocking.stubs.myMedicalRecord.emis.ProblemsStubs
import mocking.stubs.myMedicalRecord.emis.TestResultsStubs
import mocking.stubs.myMedicalRecord.tpp.PatientOverviewTpp
import mocking.stubs.myMedicalRecord.tpp.PatientSelectedTPP
import mocking.stubs.myMedicalRecord.tpp.RequestPatientRecordTpp
import mocking.stubs.myMedicalRecord.tpp.ViewDetailedTestResultsStubsTpp
import mocking.stubs.myMedicalRecord.tpp.ViewTestResultsStubsTpp

class MedicalRecordStubs(private val mockingClient: MockingClient) {

    fun generateStubs(supplier: String){
        when(supplier){
            "EMIS" -> generateEMISStubs()
            "TPP" -> generateTPPStubs()
        }
    }

    private fun generateEMISStubs(){
        TestResultsStubs(mockingClient).generateEMISStubs()
        ImmunisationsStubs(mockingClient).generateEMISStubs()
        AllergiesStubs(mockingClient).generateEMISStubs()
        MedicationsStubs(mockingClient).generateEMISStubs()
        ConsultationsStubs(mockingClient).generateEMISStubs()
        ProblemsStubs(mockingClient).generateEMISStubs()
        DemographicsStubs(mockingClient).generateEMISStubs()
    }

    private fun generateTPPStubs(){
        PatientSelectedTPP(mockingClient).generateTPPStubs()
        PatientOverviewTpp(mockingClient).generateTPPStubs()
        RequestPatientRecordTpp(mockingClient).generateTPPStubs()
        ViewTestResultsStubsTpp(mockingClient).generateTPPStubs()
        ViewDetailedTestResultsStubsTpp(mockingClient).generateTPPStubs()
    }

}