package features.sharedSteps

import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert

class SerenityHelpers {

    companion object {

        fun setPatient(patientToSet: Patient) {
            if (Serenity.hasASessionVariableCalled(Patient::class)) {
                val currentPatient = getPatient()
                Assert.assertEquals("Test setup incorrect, expected patients to be the same",
                        currentPatient.firstName,
                        patientToSet.firstName)
            } else {
                Serenity.setSessionVariable(Patient::class).to(patientToSet)
            }
        }

        fun getPatient(): Patient {
            Assert.assertTrue("Test setup incorrect, patient to be set",
                    Serenity.hasASessionVariableCalled(Patient::class))
            return Serenity.sessionVariableCalled<Patient>(Patient::class)
        }

        fun getPatientOrNull(): Patient? {
            return getValueOrNull<Patient>(Patient::class)
        }

        fun getMockingClient(): MockingClient {
            val mockingClient = getValueOrNull<MockingClient>(MockingClient::class)
            if (mockingClient != null) {
                return mockingClient
            }
            val newMockingClient = MockingClient.instance
            Serenity.setSessionVariable(MockingClient::class).to(newMockingClient)
            return newMockingClient
        }

        private fun <T>getValueOrNull(key: Any): T? {
            if (Serenity.hasASessionVariableCalled(key) ){
                return Serenity.sessionVariableCalled<T>(key)
            }
            return null
        }
    }
}
