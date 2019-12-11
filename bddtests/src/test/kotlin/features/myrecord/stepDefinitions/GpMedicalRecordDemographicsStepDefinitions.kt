package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.MyRecordInfoPage
import pages.text
import utils.SerenityHelpers

open class GpMedicalRecordDemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Then("^I see the expected demographics information - GP Medical Record$")
    fun thenISeeExpectedDemographicsGpMedicalRecord() {
        val patient = SerenityHelpers.getPatient()

        Assert.assertEquals(patient.formattedFullName(), myRecordInfoPage.patientName.text)
        Assert.assertEquals(patient.formattedDateOfBirth(), myRecordInfoPage.dateOfBirth.text)
        Assert.assertEquals(patient.formattedNHSNumber(), myRecordInfoPage.nhsNumber.text)
        Assert.assertEquals(patient.address.full(), myRecordInfoPage.address.text)
    }
}