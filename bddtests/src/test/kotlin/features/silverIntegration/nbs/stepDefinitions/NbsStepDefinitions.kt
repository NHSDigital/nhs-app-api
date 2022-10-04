package features.silverIntegration.nbs.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.thirdPartyProviders.nationalBookingService.NBSRequestBuilder
import models.IdentityProofingLevel
import pages.HybridPageObject
import utils.SerenityHelpers

class NbsStepDefinitions : HybridPageObject() {

    @Given("^NBS responds to requests for Appointment Bookings$")
    fun nbsRespondsToRequestsForAppointmentBookings() {
        MockingClient.instance.forNBS.mock {
            NBSRequestBuilder().appointmentBookingsRequest().respondWithPage()
        }
    }

    @Given("^I am a user who can view Book Coronavirus Vaccinations from NBS$")
    fun iAmAUserWhoCanViewBookCoronavirusVaccinationsFromNBS(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_BOOKING_NBS)
    }

    @Given("^I am a user who cannot view Book Coronavirus Vaccinations from NBS$")
    fun iAmAUserWhoCannotViewBookCoronavirusVaccinationsFromNBS() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_BOOKING_NONE)
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
