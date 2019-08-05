package mocking.tpp

class TppMappingRouter {
    val appointments = TppMappingBuilderAppointments()
    var myRecord = TppMappingBuilderMyRecord()
    var prescriptions = TppMappingBuilderPrescriptions()
    var authentication = TppMappingBuilderAuthentication()
    var requestMessages = TppMappingBuilderMessages()
}