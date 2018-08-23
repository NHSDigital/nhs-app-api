package pages

import mocking.defaults.MockDefaults
import models.Patient
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://stubs.local.bitraft.io:8080/citizenid/authorize")

class LoginStubPage : HybridPageObject() {
    fun signIn(patient: Patient = MockDefaults.patient) {
        findByXpath("//input[@name='mock_patient']").sendKeys(patient.hashCode().toString())
        findByXpath("//input[@type='submit']").click()
    }
}