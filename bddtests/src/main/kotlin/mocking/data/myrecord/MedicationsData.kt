package mocking.data.myrecord

import mocking.emis.models.MedicationItem
import mocking.emis.models.MedicationMedicalRecord
import mocking.emis.models.MedicationMixture
import mocking.emis.models.MedicationMixtureItem
import mocking.emis.models.MedicationsResponse
import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply
import java.time.LocalDateTime

object MedicationsData {
    private const val TWENTY_MONTHS: Long = 20
    private const val TEN_MONTHS: Long = 10
    private const val ONE_MONTH: Long = 1

    fun getEmisMedicationData(): MedicationsResponse {

        val now = LocalDateTime.now()
        val oneMonthAgo = now.minusMonths(ONE_MONTH).toString()
        val tenMonthsAgo = now.minusMonths(TEN_MONTHS).toString()
        val twentyMonthsAgo = now.minusMonths(TWENTY_MONTHS).toString()

        return MedicationsResponse (
                medicalRecord = MedicationMedicalRecord (
                    medication = mutableListOf(
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Acute",
                                drugStatus = "Active",
                                term = "Penecillin",
                                isMixture = false,
                                dosage = "One to be taken four times a day",
                                quantityRepresentation = "28 Capsules",
                                lastIssueDate = tenMonthsAgo
                        ),
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Acute",
                                drugStatus = "Active",
                                term = "Ibuprofen",
                                isMixture = false,
                                dosage = "One to be taken twice a day",
                                quantityRepresentation = "14 Capsules",
                                lastIssueDate = twentyMonthsAgo
                        ),
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Repeat",
                                drugStatus = "Active",
                                term = "Ibuprofen Plus",
                                isMixture = false,
                                dosage = "One to be taken once a day",
                                quantityRepresentation = "7 Capsules",
                                lastIssueDate = tenMonthsAgo
                        ),
                        MedicationItem (
                                firstIssueDate = tenMonthsAgo,
                                prescriptionType = "Repeat",
                                drugStatus = "Active",
                                term = "Amoxycillin",
                                isMixture = false,
                                dosage = "One to be taken twice a day",
                                quantityRepresentation = "14 Capsules",
                                lastIssueDate = oneMonthAgo
                        ),
                        MedicationItem (
                                firstIssueDate = tenMonthsAgo,
                                prescriptionType = "Repeat",
                                drugStatus = "Active",
                                term = "Inhaler Mix",
                                isMixture = true,
                                mixture = MedicationMixture (
                                        mixtureName = "MegaMix",
                                        constituents = mutableListOf<MedicationMixtureItem> (
                                                MedicationMixtureItem (
                                                        constituentName = "Ventolin",
                                                        strength = "150ml"
                                                ),
                                                MedicationMixtureItem (
                                                        constituentName = "Salbutanol",
                                                        strength = "200ml"
                                                )
                                        )
                                ),
                                dosage = "One to be taken once a day",
                                quantityRepresentation = "2 inhalers",
                                lastIssueDate = tenMonthsAgo
                        ),
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Repeat",
                                drugStatus = "Cancelled",
                                term = "Amoxycillin",
                                isMixture = false,
                                dosage = "One to be taken twice a day",
                                quantityRepresentation = "14 Capsules",
                                lastIssueDate = tenMonthsAgo
                        ),
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Repeat",
                                drugStatus = "Cancelled",
                                term = "Ibuprofen",
                                isMixture = false,
                                dosage = "One to be taken once a day",
                                quantityRepresentation = "7 Capsules",
                                lastIssueDate = tenMonthsAgo
                        )
                )
            )
        )
    }

    fun getEmisDefaultMedicationsModel(): MedicationsResponse {
        return MedicationsResponse(
                medicalRecord =  MedicationMedicalRecord(
                        medication = mutableListOf()
                ))
    }

    fun getTppMedicationData(): ViewPatientOverviewReply {
        
        val now = LocalDateTime.now()
        val tenMonthsAgo = now.minusMonths(TEN_MONTHS).toString()
        val twentyMonthsAgo = now.minusMonths(TWENTY_MONTHS).toString()

        return ViewPatientOverviewReply(
                drugs = mutableListOf
                (
                        ViewPatientOverviewItem
                        (
                                date = tenMonthsAgo,
                                value = "Penecillin"
                        )
                ),
                currentRepeats  = mutableListOf
                (
                        ViewPatientOverviewItem
                        (
                                date = tenMonthsAgo,
                                value = "Ventolin"
                        ),
                        ViewPatientOverviewItem
                        (
                                date = tenMonthsAgo,
                                value = "Salbutamol"
                        ),
                        ViewPatientOverviewItem
                        (
                                date = tenMonthsAgo,
                                value = "Calpol"
                        )
                ),
                pastRepeats  = mutableListOf
                (
                        ViewPatientOverviewItem
                        (
                                date = twentyMonthsAgo,
                                value = "Amoxycillin"
                        ),
                        ViewPatientOverviewItem
                        (
                                date = twentyMonthsAgo,
                                value = "Ibuprofen"
                        )
                )
        )
    }

    fun getTppDefaultMedicationsModel(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                drugs = mutableListOf<ViewPatientOverviewItem>(),
                currentRepeats = mutableListOf<ViewPatientOverviewItem>(),
                pastRepeats = mutableListOf<ViewPatientOverviewItem>()
        )
    }

    fun getVisionMedicationsData(): String {

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

    fun getEmptySetOfVisionMedicationData(): String {

        var response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }
}
