package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.MyRecordFactory
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.NavigationSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import pages.assertElementNotPresent
import pages.assertSingleElementPresent
import pages.myrecord.MedicalRecordV1Page
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.text
import utils.SerenityHelpers

open class V1MedicalRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    private lateinit var nav: NavigationSteps
    private lateinit var medicalRecordV1Page: MedicalRecordV1Page
    private var medicalRecordStepDefinitions = MedicalRecordStepDefinitions()
    private lateinit var headerNative: HeaderNative

    @Given("^I am a (\\w+) user setup to use medical record version 1$")
    fun iAmAUserSetupToUseMedicalRecordV1(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(supplier,
                listOf(ServiceJourneyRulesConfiguration("medical record version", "1")))

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        MyRecordFactory.getForSupplier(supplier).enabledWithBlankRecord(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)
    }

    @Then("^I see a message telling me to contact my GP for (.*) information on My Record - Medical Record v1$")
    fun thenISeeAMessageTellingMeToContactMyGPV1(heading: String) {
        assertTextInSection(heading,
                "Sorry, this information isn't available through the NHS App. To access it, contact your GP surgery.")
    }

    @Then("^I do not see a message informing me to contact my GP for this information - Medical Record v1$")
    fun thenIDoNotSeeAMessageInformingMeToContactMyGP() {
        medicalRecordV1Page.noRecordsOrNoAccessParagraph.assertElementNotPresent()
    }

    @When("^I click the (.*) section on My Record - Medical Record v1$")
    fun iClickOnTheSectionV1(heading: String) {
        medicalRecordV1Page.getSection(heading).toggleShrub()
    }

    @When("^I click on the Back link on the Medical Record page - Medical Record v1$")
    fun iClickOnBackButtonOnMyRecordsPage(){
        medicalRecordV1Page.clickOnBackLink()
    }

    @Then("^I see the medical record page - Medical Record v1$")
    fun iSeeTheMedicalRecordPageV1() {
        headerNative.waitForPageHeaderText("Your GP medical record")
        Assert.assertTrue(nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
        iSeePatientInformationDetailsV1()
    }

    @Then("^I see the patient information details - Medical Record v1$")
    fun iSeePatientInformationDetailsV1() {
        val patient = SerenityHelpers.getPatient()
        val address = patient.address.full()

        medicalRecordV1Page.assertLabelAndValue("Date of birth", patient.formattedDateOfBirth())
        medicalRecordV1Page.assertLabelAndValue("Address", address)
        medicalRecordV1Page.assertLabelAndValue("NHS number", patient.formattedNHSNumber())
    }

    @Then("^I do not see patient information details - Medical Record v1$")
    fun thenIDoNotSeePatientInformationDetailsV1() {
        assertFalse("Name field was visible.", medicalRecordV1Page.isNameVisible())
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun thenISeeServiceNotOfferedByGPOrToSpecificUserOrAccessRevokedWarningMessage() {
        assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                medicalRecordV1Page.getSummaryCareNoAccessMessage())
    }

    @Then("^I see the (.*) heading on My Record - Medical Record v1$")
    fun iSeeTheHeadingOnMyRecordV1(heading: String) {
        medicalRecordV1Page.assertSectionHeaderIsVisible(heading)
    }

    @Then("^I do not see the (.*) heading on My Record - Medical Record v1$")
    fun iDoNotSeeTheHeadingOnMyRecordV1(heading: String) {
        medicalRecordV1Page.assertSectionHeaderNotPresent(heading)
    }

    @Then("^I see the (.*) section collapsed on My Record - Medical Record v1$")
    fun iSeeTheSectionCollapsedV1(heading: String) {
        medicalRecordV1Page.assertSectionCollapsed(heading)
    }

    @Then("^I see a message indicating that I have no access to view (.*) on My Record - Medical Record v1$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSectionV1(heading: String) {
        assertTextInSection(heading, "You do not currently have access to this section")
    }

    @Then("^I see an error occurred message with (.*) on My Record - Medical Record v1$")
    fun thenISeeAnErrorOccurredMessageInSectionV1(heading: String) {
        assertTextInSection(heading, "An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message indicating that I have no information recorded for (.*) on My Record - Medical Record v1$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecordedV1(heading: String) {
        assertTextInSection(heading, "No information recorded")
    }

    private fun assertTextInSection(heading: String, message: String) {
        val section = medicalRecordV1Page.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }
}
