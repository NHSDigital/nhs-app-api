package mocking.stubs

import constants.Supplier
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient

class PatientDataGenerator {
    companion object {
        var mockingClient = MockingClient.instance

        fun generatePatientData(patients: List<Patient>, gpSupplier: Supplier) {
            patients.forEach { patient ->
                CitizenIdStubs(mockingClient).createFor(patient)
                SessionCreateJourneyFactory.getForSupplier(gpSupplier,
                        MockingClient.instance).createFor(patient)
            }
        }
    }
}