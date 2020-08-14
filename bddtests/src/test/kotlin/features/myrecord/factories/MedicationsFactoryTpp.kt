package features.myrecord.factories

import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply
import models.Patient
import worker.models.myrecord.MedicationsData
import java.time.LocalDateTime

class MedicationsFactoryTpp: MedicationsFactory() {
    override fun respondWithBadData(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithCorruptedContent()
        }
    }

    override fun getExpectedMedications(): MedicationsData {
        throw UnsupportedOperationException()
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(getTppDefaultMedicationsModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(getTppMedicationData())
        }
    }
    private fun getTppMedicationData(): ViewPatientOverviewReply {

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
                        ),
                        ViewPatientOverviewItem
                        (
                                date = twentyMonthsAgo,
                                value = "Penecillin"
                        )
                ),
                currentRepeats = mutableListOf
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
                pastRepeats = mutableListOf
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

    private fun getTppDefaultMedicationsModel(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                drugs = mutableListOf(),
                currentRepeats = mutableListOf(),
                pastRepeats = mutableListOf()
        )
    }
}
