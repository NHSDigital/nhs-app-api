package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.DataTable
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.MyRecordFactory
import features.sharedSteps.BrowserSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.assertIsVisible
import pages.gpMedicalRecord.MedicalRecordV2Page
import pages.text
import utils.SerenityHelpers
import utils.toSingleElementList

open class V2MedicalRecordStepDefinitions {

    private lateinit var medicalRecordV2Page: MedicalRecordV2Page
    @Steps
    lateinit var browser: BrowserSteps

    @Given("^I am a (\\w+) user setup to use medical record version 2$")
    fun iAmAUserSetupToUseMedicalRecordV2(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setGpSupplier(supplier)

        SerenityHelpers.setPatient(Patient.getDefault(supplier))
        SerenityHelpers.setGpSupplier(supplier)
        val patient = SerenityHelpers.getPatient()

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
        MyRecordFactory.getForSupplier(supplier).enabledWithBlankRecord(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)
    }

    @When("^I click the (.*) link on my record - Medical Record v2$")
    fun iClickTheSectionLinkOnMyRecordPageV2(linkText: String) {
        medicalRecordV2Page.clickMedicalRecordSectionLink(linkText)
    }

    @Then("^I see the medical record v2 page$")
    fun thenISeeTheMedicalRecordV2Page(){
        medicalRecordV2Page.pageTitle.assertIsVisible()
        medicalRecordV2Page.clinicalAbbreviationsLink.assertIsVisible()
    }

    @Then("^I see a message telling me to contact my GP for information on My Record - Medical Record v2$")
    fun thenISeeAMessageTellingMeToContactMyGpV2() {
        assertTextOnPage(
                "Sorry, this information isn't available through the NHS App. To access it, contact your GP surgery.")
    }

    @Then("^I see the (.*) section link on my record - Medical Record v2$")
    fun thenISeeTheSectionLinkOnMyRecordV2(linkText: String) {
        medicalRecordV2Page.assertMedicalRecordSectionLinkExists(linkText)
    }

    @Then("^I see an error occurred message on My Record - Medical Record v2$")
    fun thenISeeAnErrorOccurredMessageForProblemsV2() {
        assertTextOnPage("An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message that I have no information recorded for a specific record - Medical Record v2$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecordedV2() {
        assertTextOnPage( "No information recorded")
    }

    @Then("^I see a message that this information isn't available through the NHS App - Medical Record v2$")
    fun thenISeeAMessageIndicatingThatInformationIsntAvailable() {
        assertTextOnPage("Sorry, this information isn't available through the NHS App. To access it, "+
        "contact your GP surgery.")
    }

    @Then("^I see a message indicating that I have no access to view this section on My Record" +
            " - Medical Record v2$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSection() {
        assertTextOnPage("You do not currently have access to this section")
    }

    @Then("^retrieving the Medical Record pages directly displays the Medical Record main page$")
    fun retrievingTheMedicalRecordPageDirectlyDisplaysTheCorrectTitle(table: DataTable) {
        table.toSingleElementList().forEach { url ->
            browser.browseToInternal(url)
            val patient = SerenityHelpers.getPatient()
            medicalRecordV2Page.pageTitle.waitForElement()
            medicalRecordV2Page.assertDemographicsContent(patient)
        }
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun thenISeeServiceNotOfferedByGPOrToSpecificUserOrAccessRevokedWarningMessage() {
        Assert.assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                medicalRecordV2Page.getSummaryCareNoAccessMessage())
    }

    private fun assertTextOnPage(message: String) {
        val section = medicalRecordV2Page.getBody(message)
        assertEquals(message, section.text)
    }
}
