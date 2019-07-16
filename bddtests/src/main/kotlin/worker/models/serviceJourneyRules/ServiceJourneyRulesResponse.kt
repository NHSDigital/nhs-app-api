
@file:Suppress("EnumNaming", "Must be lower case")
package worker.models.serviceJourneyRules

data class ServiceJourneyRulesResponse( var journeys : Journey)

data class Journey(var appointments: AppointmentsJourneyRules,
                   var cdssAdvice: CdssJourneyRules,
                   var cdssAdmin: CdssJourneyRules)

data class AppointmentsJourneyRules(var informaticaUrl:String, var provider:AppointmentsProvider)

data class CdssJourneyRules(var serviceDefinition:String, var provider:CdssProvider)

enum class AppointmentsProvider{
    im1,
    informatica
}

enum class CdssProvider{
    eConsult
}