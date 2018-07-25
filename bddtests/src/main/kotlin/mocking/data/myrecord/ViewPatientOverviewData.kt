package mocking.data.myrecord

import mocking.tpp.models.ViewPatientOverviewReply

object ViewPatientOverviewData {

    fun getTppViewPatientOverviewData(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                drugs = mutableListOf(),
                currentRepeats  = mutableListOf(),
                pastRepeats  = mutableListOf()
        )
    }
}
