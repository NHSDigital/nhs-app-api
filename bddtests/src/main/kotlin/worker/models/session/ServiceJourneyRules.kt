package worker.models.session

data class ServiceJourneyRules( var journeys : Journey)

data class Journey(var appointments: AppointmentsJourneyRules,
                   var cdssAdvice: CdssJourneyRules,
                   var cdssAdmin: CdssJourneyRules)

data class AppointmentsJourneyRules(var informaticaUrl:String, var provider:AppointmentsProvider)

data class CdssJourneyRules(var serviceDefinition:String, var provider:CdssProvider)

@Suppress("EnumNaming", "Must be lower case")
enum class AppointmentsProvider{
    im1
}
@Suppress("EnumNaming", "Must be lower case")
enum class CdssProvider{
    eConsult
}