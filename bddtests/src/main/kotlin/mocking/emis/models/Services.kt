package mocking.emis.models

data class Services(
        var appointmentsSupported: Boolean = true,
        var maxAppointments: Int = 91,
        var prescribingSupported: Boolean = true,
        var epsSupported: Boolean = false,
        var demographicsUpdateSupported: Boolean = true,
        var practicePatientCommunicationSupported: Boolean = false,
        var onlineRegistrationSupported: Boolean = true,
        var preRegistrationSupported: Boolean = true,
        var medicalRecordSupported: Boolean = true,
        var medicalRecord: MedicalRecordSettings = MedicalRecordSettings()
)
