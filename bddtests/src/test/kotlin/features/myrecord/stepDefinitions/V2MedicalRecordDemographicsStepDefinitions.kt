package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.MedicalRecordV2Page
import pages.text
import utils.SerenityHelpers

open class V2MedicalRecordDemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV2Page: MedicalRecordV2Page

    @Then("^I see the expected demographics information - Medical Record v2$")
    fun thenISeeExpectedDemographicsV2() {
        val patient = SerenityHelpers.getPatient()

        Assert.assertEquals(patient.formattedFullName(), medicalRecordV2Page.patientName.text)
        Assert.assertEquals(patient.formattedDateOfBirth(), medicalRecordV2Page.dateOfBirth.text)
        Assert.assertEquals(patient.formattedNHSNumber(), medicalRecordV2Page.nhsNumber.text)
        Assert.assertEquals(patient.address.full(), medicalRecordV2Page.address.text)
    }
}