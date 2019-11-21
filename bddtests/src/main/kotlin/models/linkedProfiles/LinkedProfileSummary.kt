package models.linkedProfiles

data class LinkedProfileSummary(
        val age: String,
        val gpPracticeName: String,
        val canBookAppointment: Boolean,
        val canOrderRepeatPrescription: Boolean,
        val canViewMedicalRecord: Boolean
)
