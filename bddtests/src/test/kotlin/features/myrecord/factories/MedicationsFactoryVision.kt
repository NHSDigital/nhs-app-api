package features.myrecord.factories

import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionGetPatientDataBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient
import java.time.LocalDateTime

class MedicationsFactoryVision: MedicationsFactory() {

    override fun enabled(patient: Patient) {
        mockPatientDateRequest(patient){
            request->request.respondWithSuccess(getVisionMedicationsData())}
    }

    override fun enabledAndNoMedicationsMock(patient: Patient) {
        mockPatientDateRequest(patient){
            request->request.respondWithSuccess(getEmptySetOfVisionMedicationData())}
    }

    private fun mockPatientDateRequest(patient:Patient, resolver: (VisionGetPatientDataBuilder) -> Mapping){
         mockingClient.forVision {
            resolver(getPatientDataRequest(
                    visionUserSession = VisionUserSession.fromPatient(patient),
                    serviceDefinition = ServiceDefinition(
                            name = VisionConstants.patientDataName,
                            version = VisionConstants.patientDataVersion),
                    view = VisionConstants.medicationsView,
                    responseFormat = VisionConstants.xmlResponseFormat
            ))
        }
    }

    private fun getVisionMedicationsData(): String {

        val now = LocalDateTime.now()
        val tenMonthsAgo = now.minusMonths(TEN_MONTHS).toString()
        val oneMonthAgo = now.minusMonths(ONE_MONTH).toString()

        val currentRepeatMedication = "<clinical _category=\"Intervention\" _drugsource=\"In practice\" " +
                "_eventdate=\"1\" _first_prescribed_date=\"1\" _last_issue=\"1\" _last_prescribed_date=\"1\" " +
                "_repeat_issue_dates=\"1\" _source_id=\"421\" audited=\"0\" auditflag=\"1\" auditseq=\"827602937\" " +
                "authorise=\"0\" bnf_code=\"04070100\" category=\"SED004\" clinician=\"8\" consult_id=\"109016\" " +
                "dmd_code=\"4621811000001105\" dosage=\"1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\" " +
                "drug_code=\"79012020\" drug_dmd=\"4621811000001105\" drug_term=\"Panadol ActiFast 500mg tablets " +
                "(GlaxoSmithKline Consumer Healthcare)\" drugname=\"Panadol ActiFast 500mg tablets (GlaxoSmithKline " +
                "Consumer ...\" drugsource=\"DRG005\" entity_id=\"128\" entity_ty=\"40\" " +
                "eventdate=\"2018-10-08T00:00:00\" extracted_at_ts=\"2018-10-09T12:42:10\" " +
                "first_prescribed_date=\"2018-10-08T00:00:00\" form=\"tab\" identity=\"827602937\" inactive=\"0\" " +
                "inpractice=\"1\" iosclaim=\"0\" last_issue=\"2018-10-09T00:00:00\" " +
                "last_prescribed_date=\"$tenMonthsAgo\" master_id=\"4301\" max_issues=\"100\" num_issues=\"1\" " +
                "operat_id=\"16\" packsize=\"tablet\" parent_id=\"0\" prac_id=\"X00100\" practadmin=\"0\" " +
                "precribno=\"0\" private=\"0\" quantity=\"45\" read_code=\"di2T.00\" " +
                "repeat_issue_dates=\"2018-10-09\" repeat_ty=\"R\" rightfp10=\"1\" sensitive_read_code=\"0\" " +
                "strength=\"500\" subgroup_code=\"CurrentRepeat\" synch_id=\"128\" sysdate=\"2018-10-08T11:22:52\" " +
                "term_id=\"3001\" topic=\"1\"/>\n"

        val acuteMedication = "<clinical _category=\"Intervention\" _drugsource=\"In practice\" _eventdate=\"1\" " +
                "_first_prescribed_date=\"1\" _last_issue=\"1\" _last_prescribed_date=\"1\" " +
                "_source_id=\"421\" audited=\"0\" auditflag=\"1\" auditseq=\"827602937\" authorise=\"0\" " +
                "bnf_code=\"04070100\" category=\"SED004\" clinician=\"8\" consult_id=\"109016\" " +
                "dmd_code=\"4621811000001105\" dosage=\"1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\" " +
                "drug_code=\"79012020\" drug_dmd=\"4621811000001105\" " +
                "drug_term=\"Panadol ActiFast 500mg tablets (GlaxoSmithKline Consumer Healthcare)\" " +
                "drugname=\"Panadol ActiFast 500mg tablets (GlaxoSmithKline Consumer ...\" drugsource=\"DRG005\" " +
                "entity_id=\"128\" entity_ty=\"40\" eventdate=\"$tenMonthsAgo\" " +
                "extracted_at_ts=\"2018-10-09T12:42:10\" first_prescribed_date=\"2018-10-08T00:00:00\" " +
                "identity=\"827602937\" inactive=\"0\" master_id=\"4301\" " +
                "max_issues=\"100\" num_issues=\"1\" operat_id=\"16\" packsize=\"tablet\" parent_id=\"0\" " +
                "prac_id=\"X00100\" practadmin=\"0\" precribno=\"0\" private=\"0\" quantity=\"45\" " +
                "read_code=\"di2T.00\" repeat_issue_dates=\"2018-10-09\" repeat_ty=\"R\" rightfp10=\"1\" " +
                "sensitive_read_code=\"0\" strength=\"500\" subgroup_code=\"Acute\" synch_id=\"128\" " +
                "sysdate=\"2018-10-08T11:22:52\" term_id=\"3001\" topic=\"1\"/>\n"

        val pastRepeatMedication = "<clinical _category=\"Intervention\" _drugsource=\"In practice\" " +
                "_eventdate=\"1\"  _first_prescribed_date=\"1\" _last_issue=\"1\" _last_prescribed_date=\"1\" " +
                "_repeat_issue_dates=\"1\" _source_id=\"421\" audited=\"0\" auditflag=\"1\" auditseq=\"827602937\" " +
                "authorise=\"0\" bnf_code=\"04070100\" category=\"SED004\" clinician=\"8\" consult_id=\"109016\" " +
                "dmd_code=\"4621811000001105\" dosage=\"1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\" " +
                "drug_code=\"79012020\" drug_dmd=\"4621811000001105\" drug_term=\"Panadol ActiFast 500mg tablets " +
                "(GlaxoSmithKline Consumer Healthcare)\" drugname=\"Panadol ActiFast 500mg tablets (GlaxoSmithKline " +
                "Consumer ...\" drugsource=\"DRG005\" entity_id=\"128\" entity_ty=\"40\" " +
                "eventdate=\"2018-10-08T00:00:00\" extracted_at_ts=\"2018-10-09T12:42:10\" " +
                "first_prescribed_date=\"2018-10-08T00:00:00\" form=\"tab\" identity=\"827602937\" inactive=\"0\" " +
                "inpractice=\"1\" iosclaim=\"0\" last_issue=\"2018-10-09T00:00:00\" " +
                "last_prescribed_date=\"$oneMonthAgo\" master_id=\"4301\" max_issues=\"100\" num_issues=\"1\" " +
                "operat_id=\"16\" packsize=\"tablet\" parent_id=\"0\" prac_id=\"X00100\" practadmin=\"0\" " +
                "precribno=\"0\" private=\"0\" quantity=\"45\" read_code=\"di2T.00\" " +
                "repeat_issue_dates=\"2018-10-09\" repeat_ty=\"R\" rightfp10=\"1\" sensitive_read_code=\"0\" " +
                "strength=\"500\" subgroup_code=\"DiscontinuedRepeat\" sysdate=\"2018-10-08T11:22:52\" " +
                "term_id=\"3001\" topic=\"1\"/>\n"

        var response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        response += currentRepeatMedication
        response += acuteMedication
        response += pastRepeatMedication

        return response + responseStringEnd
    }

    private fun getEmptySetOfVisionMedicationData(): String {

        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }
}