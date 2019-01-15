package features.organDonation.stepDefinitions

import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.models.Mapping
import mocking.organDonation.OrganDonationLookupRegistrationBuilder
import mocking.organDonation.OrganDonationSubmitDecisionBuilder
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationDecision
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.Patient
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers

class OrganDonationFactory(val gpSystem: String) {

    val mockingClient = MockingClient.instance

    val patient = setupPatient()

    fun setupPatientForAppUse() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        mockingClient.forOrganDonation {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
        }
    }

    fun lookUpRegistrationWithSuccessfulDemographics(patient:Patient? = null,
                                                     action: (OrganDonationLookupRegistrationBuilder) -> Mapping) {
        val patientToUse = patient ?: setupPatient()
        mockingClient.forOrganDonation {
            action(lookupOrganDonationRegistration(patientToUse))
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patientToUse)
    }

    fun demographicsTimeout() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabledButTimesOut(patient)
    }

    fun demographicsInternalError() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).throwInternalError(patient)
    }

    fun optOut(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val patient = setupPatient()
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        val optOutRegistration = OrganDonationRegistrationRequest(OrganDonationRegistration.fromPatient(patient),
                OrganDonationAdditionalDetails())
        optOutRegistration.registration.decision = OrganDonationRegistrationDecision.OptOut
        Serenity.setSessionVariable("OrganDonationDecision").to(optOutRegistration)
        mockingClient.forOrganDonation { action(submitDecision(optOutRegistration))}
    }

    private fun setupPatient(): Patient {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        return patient
    }
}