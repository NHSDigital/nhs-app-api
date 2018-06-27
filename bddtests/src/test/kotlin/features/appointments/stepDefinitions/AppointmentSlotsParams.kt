package features.appointments.stepDefinitions

import mocking.defaults.MockDefaults
import models.Patient

data class AppointmentSlotsParams(
        var patient: Patient = MockDefaults.patient,
        var sessionStartDate: String? = null,
        var sessionEndDate: String? = null
)