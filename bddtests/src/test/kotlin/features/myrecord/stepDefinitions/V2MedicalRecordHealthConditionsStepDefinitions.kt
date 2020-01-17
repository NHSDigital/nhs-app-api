package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.ProblemsFactory
import org.junit.Assert
import pages.gpMedicalRecord.HealthConditionsPage
import utils.SerenityHelpers

open class V2MedicalRecordHealthConditionsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var healthConditionsPage: HealthConditionsPage

    private val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018",
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018",
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018"
            ), Supplier.VISION to arrayOf(
                "10 October 2018\nPeanut allergy\nStatus: Past",
                "10 October 2018\nBroken leg\nStatus: Current",
                "10 October 2018\nAcne\nStatus: Random"
            ), Supplier.MICROTEST to arrayOf(
                "3 July 2019\nFinish Date: Ongoing\nRubric 1",
                "3 July 2019\nFinish Date: Ongoing\nRubric 2",
                "3 July 2019\nFinish Date: Ongoing\nRubric 3"
            ))

    @Then("^I see the expected health conditions - Medical Record v2$")
    fun thenISeeExpectedHealthConditionsV2() {
        val healthConditionsMessages = healthConditionsPage.getHealthConditionsElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertEquals(expectedData[supplier]?.size, healthConditionsMessages.size)

        healthConditionsMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text) }
    }

    @Given("the GP practice responds with bad problems data")
    fun thereIsBadDataRecievedForProblems() {
        ProblemsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                .badDataResponse(SerenityHelpers.getPatient())
    }
}
