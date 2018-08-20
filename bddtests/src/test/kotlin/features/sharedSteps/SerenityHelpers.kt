package features.sharedSteps

import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert

class SerenityHelpers {

    companion object {

        fun setPatient(patientToSet: Patient) {
            if (Serenity.hasASessionVariableCalled(Patient::class)) {
                var currentPatient = getPatient()
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
            if (Serenity.hasASessionVariableCalled(Patient::class)) {
                return Serenity.sessionVariableCalled<Patient>(Patient::class)
            }
            return null
        }
    }
}
