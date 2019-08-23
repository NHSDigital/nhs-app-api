@file:Suppress("EnumNaming", "Must be lower case")
package worker.models.serviceJourneyRules

enum class AppointmentsProvider{
    im1,
    informatica,
    gpAtHand
}

enum class CdssProvider{
    none,
    eConsult
}

enum class MedicalRecordProvider{
    im1,
    gpAtHand
}

enum class PrescriptionsProvider{
    im1,
    gpAtHand
}