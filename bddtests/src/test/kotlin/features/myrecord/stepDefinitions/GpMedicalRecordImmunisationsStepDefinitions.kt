package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.ImmunisationsFactory
import features.myrecord.factories.MyRecordFactory
import mocking.data.myrecord.ImmunisationsData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import org.junit.Assert
import pages.gpMedicalRecord.ImmunisationsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers

open class GpMedicalRecordImmunisationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var immunisationsPage: ImmunisationsPage

    val expectedData = mapOf(
            "EMIS" to arrayOf(
                "18 February 2018\nSecond meningitis C Vaccination",
                "15 May 2002\nFirst meningitis C Vaccination"
            ), "VISION" to arrayOf(
                "10 October 2018\nLumpectomy NEC",
                "10 October 2018\nLumpectomy NEC"
            ), "MICROTEST" to arrayOf(
                "3 July 2019\nImmunisation 1\nNext Date: no next date\nStatus: Main 1",
                "3 July 2019\nImmunisation 2\nNext Date: no next date\nStatus: Main 2",
                "3 July 2019\nImmunisation 3\nNext Date: no next date\nStatus: Main 3"
            ))

    @Given("^the GP Practice has enabled immunisations functionality" +
            " and multiple immunisation records exist - GP Medical Record$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleRecordsExist() {
        val supplier = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(supplier).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has enabled immunisations functionality" +
            " and no immunisation records exist - GP Medical Record$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndNoRecordsExist() {
        val supplier = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(supplier).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the user does not have access to view immunisations - GP Medical Record$")
    fun givenUserDoesNotHaveAccessToViewImmunisations() {
        val supplier = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(supplier).noAccess(SerenityHelpers.getPatient())
    }

    @Then("^I see the expected immunisations - GP Medical Record$")
    fun thenISeeExpectedImmunisationsRecordGpMedicalRecord() {
        val immunisationsMessages = immunisationsPage.getImmunisationsElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertTrue(
                "Expected records", immunisationsMessages.size == expectedData[supplier]?.size )
        immunisationsMessages.forEachIndexed { i, message ->
            Assert.assertTrue(message.text == expectedData[supplier]?.get(i)) }
    }

    @Given("^the EMIS GP Practice has two immunisation results" +
            " where the first record has no date - GP Medical Record$")
    fun givenTheEmisGpPracticeHasAnImmunisationResultWithNoDate() {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(ImmunisationsData.getTwoImmunisationResultsWhereTheFirstRecordHasNoDate())
        }
    }

    @Then("^I see the expected immunisations with an unknown date for the first result - GP Medical Record$")
    fun thenISeeExpectedImmunisationsWithUnknownDateGpMedicalRecord() {
        val immunisationsMessages = immunisationsPage.getImmunisationsElements()

        val expectedMessages = listOf(
            "Unknown Date\nFirst meningitis C Vaccination",
            "18 February 2018\nSecond meningitis C Vaccination"
        )

        Assert.assertTrue("Expected records", immunisationsMessages.size == expectedMessages.size )
        immunisationsMessages.forEachIndexed { i, message -> Assert.assertTrue(message.text == expectedMessages[i]) }
    }

    @Given("^MICROTEST have enabled immunisations and records exist - GP Medical Record$")
    fun givenMicrotestHaveEnabledImmunisationsAndRecordsExist() {
        val supplier = "MICROTEST"

        val myRecordModuleCounts = MyRecordModuleCounts()
        myRecordModuleCounts.vaccinationsCount = expectedData["MICROTEST"]!!.size

        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(supplier).enabledWithData(
                SerenityHelpers.getPatient(),
                myRecordModuleCounts,
                TestResultOptions())
    }

    @Given("MICROTEST have enabled immunisations and no records exist - GP Medical Record")
    fun givenMicrotestHaveEnabledImmunisationsNoRecords() {
        val supplier = "MICROTEST"

        val myRecordModuleCounts = MyRecordModuleCounts()
        myRecordModuleCounts.vaccinationsCount = 0

        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(supplier).enabledWithData(
                SerenityHelpers.getPatient(),
                myRecordModuleCounts,
                TestResultOptions())
    }

}