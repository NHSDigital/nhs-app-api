package mocking.vision

class VisionMappingRouter {
    var appointments = VisionMappingBuilderAppointments()
    var prescriptions = VisionMappingBuilderPrescriptions()
    var myRecord = VisionMappingBuilderMyRecord()
    var authentication = VisionMappingBuilderAuthentication()
}