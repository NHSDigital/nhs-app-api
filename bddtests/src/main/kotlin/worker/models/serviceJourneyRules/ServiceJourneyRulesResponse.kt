
@file:Suppress("EnumNaming", "Must be lower case")
package worker.models.serviceJourneyRules

data class ServiceJourneyRulesResponse( var journeys : Journey)

data class Journey(var appointments: AppointmentsJourneyRules,
                   var cdssAdvice: CdssJourneyRules,

                   var cdssAdmin: CdssJourneyRules,
                   var medicalRecord: MedicalRecordJourneyRules,
                   var prescriptions: PrescriptionsJourneyRules,
                   var nominatedPharmacy: Boolean)

data class AppointmentsJourneyRules(var informaticaUrl:String, var provider:AppointmentsProvider)

data class CdssJourneyRules(var serviceDefinition:String, var provider:CdssProvider)

data class MedicalRecordJourneyRules(var provider:MedicalRecordProvider)

data class PrescriptionsJourneyRules(var provider:PrescriptionsProvider)
