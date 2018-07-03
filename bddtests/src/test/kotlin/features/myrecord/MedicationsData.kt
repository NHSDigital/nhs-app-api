package features.myrecord

import mocking.emis.models.*
import mocking.emis.testResults.TestResultMedicalRecord
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultResponseModel
import mocking.tpp.models.Item
import mocking.tpp.models.ViewPatientOverviewReply
import java.time.LocalDateTime

object MedicationsData {

    fun getEmisMedicationData(): MedicationsResponse {

        val now = LocalDateTime.now();
        val oneMonthAgo = now.minusMonths(1).toString();
        val tenMonthsAgo = now.minusMonths(10).toString();
        val twentyMonthsAgo = now.minusMonths(20).toString();

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

        val now = LocalDateTime.now();
        val tenMonthsAgo = now.minusMonths(10).toString();
        val twentyMonthsAgo = now.minusMonths(20).toString();

        return ViewPatientOverviewReply(
                drugs = mutableListOf
                (
                        Item
                        (
                                date = tenMonthsAgo,
                                value = "Penecillin"
                        )
                ),
                currentRepeats  = mutableListOf
                (
                        Item
                        (
                                date = tenMonthsAgo,
                                value = "Ventolin"
                        ),
                        Item
                        (
                                date = tenMonthsAgo,
                                value = "Salbutamol"
                        ),
                        Item
                        (
                                date = tenMonthsAgo,
                                value = "Calpol"
                        )
                ),
                pastRepeats  = mutableListOf
                (
                        Item
                        (
                                date = twentyMonthsAgo,
                                value = "Amoxycillin"
                        ),
                        Item
                        (
                                date = twentyMonthsAgo,
                                value = "Ibuprofen"
                        )
                )
        );
    }

    fun getTppDefaultMedicationsModel(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                drugs = mutableListOf<Item>(),
                currentRepeats = mutableListOf<Item>(),
                pastRepeats = mutableListOf<Item>()
        );
    }
}
