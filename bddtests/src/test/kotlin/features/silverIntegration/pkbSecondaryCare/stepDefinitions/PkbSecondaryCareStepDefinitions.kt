package features.silverIntegration.pkbSecondaryCare.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.pages.SecondaryCarePage
import mocking.thirdPartyProviders.pkb.SecondaryCareRequestBuilder
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import utils.SerenityHelpers

class PkbSecondaryCareStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage
    private lateinit var secondaryCarePage: SecondaryCarePage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage

    @Given("^I am a user who can view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCanViewMedicinesFromPkbSecondaryCare() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewMedicinesFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view Appointments from PKB Secondary Care$")
    fun iAmAUserWhoCanViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Appointments from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^Secondary Care responds to requests for medicines$")
    fun secondaryCareRespondsToRequestsForMedicines() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().medicineRequest().respondWithPage()
        }
    }

    @Given("^Secondary Care responds to requests for appointments$")
    fun secondaryCareRespondsToRequestsForAppointments() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().appointmentRequest().respondWithPage()
        }
    }

    @Then("^the PKB Secondary Care View Medicines link is available on the Prescriptions Hub$")
    fun thePKBSecondaryCareViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the PKB Secondary Care View Medicines link is not available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the PKB Secondary Care View Medicines link on the Prescriptions hub$")
    fun iClickThePkbSecondaryCareViewMedicinesLink(){
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.click()
    }

    @Then("^I can see the PKB Secondary Care View Appointments link on the Appointments page$")
    fun iCanSeeThePkbViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertPkbViewAppointmentsIsDisplayed()
    }

    @When("^I click the PKB Secondary Care View Appointments link on the Appointments page")
    fun iClickThePkbViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.click()
    }

    @Then("the view appointments warning on the page explains the service is from PKB Secondary Care$")
    fun assertViewAppointmentsWarningMessageContentForPkbSecondaryCare() {
        redirector.interruptionCard.assertContent(
                "View appointments\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the view appointments warning on the page explains the service is from PKB My Care View")
    fun assertViewAppointmentsWarningMessageContentForPkbMyCareView() {
        redirector.interruptionCard.assertContent(
                "View appointments\nThis service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the hospital and medicines warning on the page explains the service is from PKB Secondary Care$")
    fun assertHospitalAndMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("^the link to PKB Secondary Care View Appointments is not available on the Appointments page$")
    fun theLinkToPkbViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.assertElementNotPresent()
    }

    @Then("^I am navigated to a third party site for Secondary Care")
    fun iNavigateToThirdPartySiteForSecondaryCare() {
        secondaryCarePage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
