package features.myrecord.factories

import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply
import models.Patient
import java.time.LocalDateTime

class MedicationsFactoryTpp: MedicationsFactory() {
    override fun enabled(patient: Patient) {
        mockingClient.forTpp {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(getTppMedicationData())
        }
    }

    override fun enabledAndNoMedicationsMock(patient: Patient) {
            mockingClient.forTpp {
                myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                        .respondWithSuccess(getTppDefaultMedicationsModel())
            }
    }

    private fun getTppMedicationData(): ViewPatientOverviewReply {

        val now = LocalDateTime.now()
        val tenMonthsAgo = now.minusMonths(tenMonths).toString()
        val twentyMonthsAgo = now.minusMonths(twentyMonths).toString()

        return ViewPatientOverviewReply(
                drugs = mutableListOf
                (
                        ViewPatientOverviewItem
                        (
                                date = tenMonthsAgo,
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
                drugs = mutableListOf<ViewPatientOverviewItem>(),
                currentRepeats = mutableListOf<ViewPatientOverviewItem>(),
                pastRepeats = mutableListOf<ViewPatientOverviewItem>()
        )
    }
}