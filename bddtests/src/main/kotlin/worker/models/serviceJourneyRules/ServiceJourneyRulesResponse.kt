package worker.models.serviceJourneyRules


data class ServiceJourneyRulesResponse(
        var Appointments: Appointments
)

data class Appointments(
        var Provider: AppointmentsProvider
)

enum class AppointmentsProvider {
    Unknown,
    None,
    Im1
}
