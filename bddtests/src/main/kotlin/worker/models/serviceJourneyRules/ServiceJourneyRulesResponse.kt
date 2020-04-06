package worker.models.serviceJourneyRules

data class ServiceJourneyRulesResponse(var journeys : Journey)

data class Journey(var appointments: AppointmentsJourneyRules,
                   var cdssAdvice: CdssJourneyRules,
                   var cdssAdmin: CdssJourneyRules,
                   var medicalRecord: MedicalRecordJourneyRules,
                   var prescriptions: PrescriptionsJourneyRules,
                   var nominatedPharmacy: Boolean,
                   var silverIntegrations: SilverIntegrationJourneyRules,
                   var documents: Boolean,
                   var im1Messaging: Im1MesagingJourneyRules,
                   var homeScreen: HomeScreen)

data class AppointmentsJourneyRules(var informaticaUrl:String, var provider:AppointmentsProvider)

data class CdssJourneyRules(var serviceDefinition:String, var provider:CdssProvider)

data class MedicalRecordJourneyRules(var provider:MedicalRecordProvider, var version: Number?)

data class PrescriptionsJourneyRules(var provider:PrescriptionsProvider)

data class SilverIntegrationJourneyRules(var secondaryAppointments:ArrayList<SecondaryAppointmentsProvider>,
                                         var messages: ArrayList<MessagesProvider>,
                                         var consultations: ArrayList<ConsultationsProvider>)

data class PublicHealthNotification(
        var id: String,
        var type: PublicHealthNotificationType,
        var urgency: PublicHealthNotificationUrgency,
        var title: String,
        var body: String
)

data class HomeScreen(var publicHealthNotifications: ArrayList<PublicHealthNotification>)

data class Im1MesagingJourneyRules(var isEnabled: Boolean,
                                   var canDeleteMessages: Boolean,
                                   var canUpdateReadStatus: Boolean,
                                   val requiresDetailsRequest: Boolean)
