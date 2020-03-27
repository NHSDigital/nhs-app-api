package mocking.tpp
import mocking.tpp.listServiceAccesses.TppListServiceAccessesBuilder
import models.Patient

class TppMappingRouter {
    val appointments = TppMappingBuilderAppointments()
    var myRecord = TppMappingBuilderMyRecord()
    var prescriptions = TppMappingBuilderPrescriptions()
    var authentication = TppMappingBuilderAuthentication()
    var requestMessages = TppMappingBuilderMessages()
    var patientPracticeMessaging = TppMappingBuilderPatientPracticeMessaging()
    fun listServiceAccesses(patient: Patient) = TppListServiceAccessesBuilder(patient.tppUserSession!!)
}