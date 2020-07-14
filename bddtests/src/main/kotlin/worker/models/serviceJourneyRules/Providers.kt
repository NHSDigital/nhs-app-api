@file:Suppress("EnumNaming", "Must be lower case")
package worker.models.serviceJourneyRules

enum class AppointmentsProvider{
    im1,
    informatica,
    gpAtHand
}

enum class CdssProvider{
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

enum class SecondaryAppointmentsProvider {
    ers,
    pkb
}

enum class MessagesProvider {
    pkb,
    testSilverThirdPartyProvider
}

enum class MedicinesProvider {
    pkb
}

enum class ConsultationsProvider {
    pkb
}

enum class PublicHealthNotificationType {
    callout
}

enum class PublicHealthNotificationUrgency {
    warning
}
