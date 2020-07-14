package features.gpAtHand.stepDefinitions

import cucumber.api.java.en.Then
import pages.gpAtHand.GpAtHandAppointmentsPage
import pages.gpAtHand.GpAtHandMedicalRecordPage
import pages.gpAtHand.GpAtHandPrescriptionsPage

class GpAtHandStepDefinitions {

    private lateinit var gpAtHandAppointmentsPage: GpAtHandAppointmentsPage
    private lateinit var gpAtHandMedicalRecordPage: GpAtHandMedicalRecordPage
    private lateinit var gpAtHandPrescriptions : GpAtHandPrescriptionsPage

    @Then("^I see an appropriate message informing me that my GP surgery uses the Babylon App for appointments$")
    fun iSeeAnAppropriateMessageInformingMeThatMyGpSurgeryUsesTheBabylonAppForAppointments() {
        gpAtHandAppointmentsPage.isLoaded()
        gpAtHandAppointmentsPage.assertGpAtHandPageVisible()
    }

    @Then("^I see an appropriate message informing me that my GP surgery uses the Babylon App for my medical record$")
    fun iSeeAnAppropriateMessageInformingMeThatMyGpSurgeryUsesTheBabylonAppForMedicalRecord() {
        gpAtHandMedicalRecordPage.isLoaded()
        gpAtHandMedicalRecordPage.assertGpAtHandPageVisible()
    }

    @Then("^I see an appropriate message informing me that my GP surgery uses the Babylon App for prescriptions$")
    fun iSeeAnAppropriateMessageInformingMeThatMyGpSurgeryUsesTheBabylonAppForPrescriptions() {
        gpAtHandPrescriptions.isLoaded()
        gpAtHandPrescriptions.assertGpAtHandPageVisible()
    }
}
