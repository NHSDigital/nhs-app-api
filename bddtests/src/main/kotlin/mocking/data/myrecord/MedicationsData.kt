package mocking.data.myrecord

import mocking.emis.models.MedicationItem
import mocking.emis.models.MedicationMedicalRecord
import mocking.emis.models.MedicationMixture
import mocking.emis.models.MedicationMixtureItem
import mocking.emis.models.MedicationsResponse
import java.time.LocalDateTime

object MedicationsData {
    private const val TWENTY_MONTHS: Long = 20
    private const val TEN_MONTHS: Long = 10
    private const val ONE_MONTH: Long = 1

    val now = LocalDateTime.now()
    val oneMonthAgo = now.minusMonths(ONE_MONTH).toString()
    val tenMonthsAgo = now.minusMonths(TEN_MONTHS).toString()
    val twentyMonthsAgo = now.minusMonths(TWENTY_MONTHS).toString()

    fun getEmisMedicationData(): MedicationsResponse {

        return MedicationsResponse (
                medicalRecord = MedicationMedicalRecord (
                    medication = mutableListOf(
                        MedicationItem (
                                firstIssueDate = twentyMonthsAgo,
                                prescriptionType = "Acute",
                                drugStatus = "Active",
                                term = "Penicillin",
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

    fun getEmisAcuteMedicationsResponseWhereTheFirstResultHasNoDate(): MedicationsResponse {
        return MedicationsResponse (
                medicalRecord = MedicationMedicalRecord (
                        medication = mutableListOf(
                                MedicationItem (
                                        firstIssueDate = "",
                                        prescriptionType = "Acute",
                                        drugStatus = "Active",
                                        term = "Amoxicillin",
                                        isMixture = false,
                                        dosage = "One to be taken four times a day",
                                        quantityRepresentation = "28 Capsules",
                                        lastIssueDate = ""
                                ),
                                MedicationItem (
                                        firstIssueDate = "",
                                        prescriptionType = "Acute",
                                        drugStatus = "Active",
                                        term = "Penicillin",
                                        isMixture = false,
                                        dosage = "One to be taken twice a day",
                                        quantityRepresentation = "14 Capsules",
                                        lastIssueDate = tenMonthsAgo
                                ),
                                MedicationItem (
                                        firstIssueDate = tenMonthsAgo,
                                        prescriptionType = "Acute",
                                        drugStatus = "Active",
                                        term = "Ibuprofen",
                                        isMixture = false,
                                        dosage = "One to be taken twice a day",
                                        quantityRepresentation = "14 Capsules",
                                        lastIssueDate = tenMonthsAgo
                                )
                        )
                )
        )
    }
    
    fun getEmisCurrentRepeatMedicationsResponseWhereTheFirstResultHasNoDate(): MedicationsResponse {
        return MedicationsResponse (
                medicalRecord = MedicationMedicalRecord (
                        medication = mutableListOf(
                                MedicationItem (
                                        firstIssueDate = "",
                                        prescriptionType = "Repeat",
                                        drugStatus = "Active",
                                        term = "Penicillin",
                                        isMixture = false,
                                        dosage = "One to be taken four times a day",
                                        quantityRepresentation = "28 Capsules",
                                        lastIssueDate = tenMonthsAgo
                                ),
                                MedicationItem (
                                        firstIssueDate = tenMonthsAgo,
                                        prescriptionType = "Repeat",
                                        drugStatus = "Active",
                                        term = "Ibuprofen",
                                        isMixture = false,
                                        dosage = "One to be taken twice a day",
                                        quantityRepresentation = "14 Capsules",
                                        lastIssueDate = tenMonthsAgo

                                )
                        )
                )
        )
    }

    fun getEmisDiscontinuedRepeatMedicationsResponseWhereTheFirstResultHasNoDate(): MedicationsResponse {
        return MedicationsResponse (
                medicalRecord = MedicationMedicalRecord (
                        medication = mutableListOf(
                                MedicationItem (
                                        firstIssueDate = "",
                                        prescriptionType = "Repeat",
                                        drugStatus = "Cancelled",
                                        term = "Penicillin",
                                        isMixture = false,
                                        dosage = "One to be taken four times a day",
                                        quantityRepresentation = "28 Capsules",
                                        lastIssueDate = tenMonthsAgo
                                ),
                                MedicationItem (
                                        firstIssueDate = twentyMonthsAgo,
                                        prescriptionType = "Repeat",
                                        drugStatus = "Cancelled",
                                        term = "Ibuprofen",
                                        isMixture = false,
                                        dosage = "One to be taken twice a day",
                                        quantityRepresentation = "14 Capsules",
                                        lastIssueDate = twentyMonthsAgo
                                )
                        )
                )
        )
    }
}
