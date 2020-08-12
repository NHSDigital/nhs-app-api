package features.myrecord.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.myrecord.factories.ProblemsFactory
import org.junit.Assert
import pages.gpMedicalRecord.HealthConditionsPage
import utils.SerenityHelpers

open class V2MedicalRecordHealthConditionsStepDefinitions {

    private lateinit var healthConditionsPage: HealthConditionsPage

    private val expectedData = mapOf(
            Supplier.EMIS to arrayOf(
                "17 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018",
                "16 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018",
                "15 May 2018\nConjunctivitis\nSignificance: Minor\nStatus: Past\n" +
                        "Notes:\nPatient advice given\nRepeated use of eye drops\nEnded: 15 May 2018"
            ), Supplier.VISION to arrayOf(
                "10 October 2018\nPeanut allergy\nStatus: Past",
                "10 October 2018\nBroken leg\nStatus: Current",
                "10 October 2018\nAcne\nStatus: Random"
            ), Supplier.MICROTEST to arrayOf(
                "5 July 2019\nFinish Date: Ongoing\nRubric 3",
                "4 July 2019\nFinish Date: Ongoing\nRubric 2",
                "3 July 2019\nFinish Date: Ongoing\nRubric 1"
            ))

        @Then("^I see the expected health conditions - Medical Record v2$")
        fun thenISeeExpectedHealthConditionsV2() {
        val healthConditionsMessages = healthConditionsPage.getHealthConditionsElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertEquals(expectedData[supplier]?.size, healthConditionsMessages.size)

        healthConditionsMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text) }
    }

    @Given("^the GP practice responds with bad problems data$")
    fun thereIsBadDataReceivedForProblems() {
        ProblemsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                .badDataResponse(SerenityHelpers.getPatient())
    }

    @Then("^I see the problem with unknown date displayed last - Medical Record v2$")
    fun thenISeeTheProblemWithUnknownDateDisplayedLastV2() {
        val healthConditionsMessages = healthConditionsPage.getHealthConditionsElements()

        val dataWithUnknownDate = expectedData[Supplier.EMIS]
        dataWithUnknownDate?.set(2, dataWithUnknownDate[1].replace("16 May 2018", "Unknown Date"))
        dataWithUnknownDate?.set(1, dataWithUnknownDate[1].replace("16 May 2018", "15 May 2018"))

        Assert.assertEquals(healthConditionsMessages.size, dataWithUnknownDate?.size)

        healthConditionsMessages.forEachIndexed { i, message ->
            Assert.assertEquals(message.text, dataWithUnknownDate?.get(i)) }
    }
}
