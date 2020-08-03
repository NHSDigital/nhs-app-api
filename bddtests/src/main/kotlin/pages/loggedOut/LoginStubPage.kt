package pages.loggedOut

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject

@DefaultUrl("http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/authorize")

class LoginStubPage : HybridPageObject() {

    fun signIn(patient: Patient) {
        findByXpath("//input[@name='mock_patient']").sendKeys(patient.hashCode().toString())
        findByXpath("//input[@type='submit']").click()
    }
}
