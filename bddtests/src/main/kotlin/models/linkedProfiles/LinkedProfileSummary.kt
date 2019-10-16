package models.linkedProfiles

data class LinkedProfileSummary(
        val nhsNumber: String,
        val dateOfBirth: String,
        val gpPracticeName: String,
        val canBookAppointment: Boolean,
        val canOrderRepeatPrescription: Boolean,
        val canViewMedicalRecord: Boolean
)
