package mocking.emis

import mocking.emis.practices.PracticeSettingsBuilderEmis
import models.Patient

class EmisMappingRouter(configuration: EmisConfiguration?) {
    var appointments = EmisMappingBuilderAppointments(configuration)
    var myRecord = EmisMappingBuilderMyRecord(configuration)
    var messaging = EmisMappingBuilderMessages(configuration)
    var myRecordProxy = EmisProxyMappingBuilderMyRecord(configuration)
    var prescriptions = EmisMappingBuilderPrescriptions(configuration)
    var authentication = EmisMappingBuilderAuthentication(configuration)
    fun practiceSettingsRequest(patient: Patient) = PracticeSettingsBuilderEmis(patient)
}
