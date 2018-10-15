package mocking.vision.models

import models.Patient

data class VisionUserSession(
        var rosuAccountId: String,
        var apiKey: String,
        var odsCode: String,
        var patientId: String,
        var isRepeatPrescriptionsEnabled: Boolean = true) {
    var provider: String
    var accountId: String

    init {
        val defaultProviderAccountId = "nhson001"
        provider = defaultProviderAccountId
        accountId = defaultProviderAccountId
    }

    companion object {

        fun fromPatient(patient: Patient): VisionUserSession {
            return VisionUserSession(
                    patient.rosuAccountId,
                    patient.apiKey,
                    patient.odsCode,
                    patient.patientId)
        }
    }
}