package mocking.stubs

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient

class PatientDataGenerator {
    companion object {
        var mockingClient = MockingClient.instance

        fun generatePatientData(patients: List<Patient>, supplier: String ) {
            patients.forEach { patientDetails ->
                CitizenIdStubs(mockingClient).createFor(patientDetails)
                SessionCreateJourneyFactory.getForSupplier(supplier, MockingClient.instance).createFor(patientDetails)
            }
        }
    }
}