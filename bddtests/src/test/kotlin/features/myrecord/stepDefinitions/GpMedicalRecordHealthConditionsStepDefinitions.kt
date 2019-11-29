package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Then
import org.junit.Assert
import pages.gpMedicalRecord.HealthConditionsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers

open class GpMedicalRecordHealthConditionsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    private lateinit var healthConditionsPage: HealthConditionsPage

    private val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given Repeated use of eye drops\nEnded: 15 May 2018",
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given Repeated use of eye drops\nEnded: 15 May 2018",
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given Repeated use of eye drops\nEnded: 15 May 2018"
            ), Supplier.VISION to arrayOf(
                "10 October 2018\nPeanut allergy\nStatus: Past",
                "10 October 2018\nBroken leg\nStatus: Current",
                "10 October 2018\nAcne\nStatus: Random"
            ), Supplier.MICROTEST to arrayOf(
                "3 July 2019\nFinish Date: Ongoing\nRubric 1",
                "3 July 2019\nFinish Date: Ongoing\nRubric 2",
                "3 July 2019\nFinish Date: Ongoing\nRubric 3"
            ))

    @Then("^I see the expected health conditions - GP Medical Record$")
    fun thenISeeExpectedHealthConditionsGpMedicalRecord() {
        val healthConditionsMessages = healthConditionsPage.getHealthConditionsElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertEquals(healthConditionsMessages.size, expectedData[supplier]?.size )

        healthConditionsMessages.forEachIndexed { i, message ->
            Assert.assertEquals(message.text, expectedData[supplier]?.get(i)) }
    }
}