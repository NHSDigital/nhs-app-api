package features.organDonation.stepDefinitions

import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.models.Mapping
import mocking.organDonation.OrganDonationLookupRegistrationBuilder
import mocking.organDonation.models.Resource
import mocking.organDonation.models.OrganDonationDemographics
import models.Patient
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

    private fun existing(registration: Resource) {
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun existingOptOut(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient, organDonationDemographics)
        existing(registration)
        return registration
    }

    fun existingOptIn(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optIn(patient, organDonationDemographics)
        existing(registration)
        return registration
    }

    fun existingAppointedRepresentative() {
        val registration = OrganDonationRegistrationDataBuilder.appointRepresentative(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun existingOptInSome(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optInSome(patient, organDonationDemographics)
        existing(registration)
        return registration
    }

    fun existingOptInSomeNotAllDecided(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optInSomeNotAllDecided(patient,
                organDonationDemographics)
        existing(registration)
        return registration
    }

    fun lookUpRegistrationWithSuccessfulDemographics(patient: Patient? = null,
                                                     action: (OrganDonationLookupRegistrationBuilder) -> Mapping) {
        val patientToUse = patient ?: setupPatient()
        mockingClient.forOrganDonation {
            action(lookupOrganDonationRegistration(patientToUse))
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patientToUse)
    }

    fun demographicsTimeout() {
        val patient = setupPatient()
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabledButTimesOut(patient)
    }

    fun demographicsInternalError() {
        val patient = setupPatient()
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).throwInternalError(patient)
    }

    private fun setupPatient(): Patient {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        return patient
    }

    fun amend(action: (OrganDonationAmendCreateFactory) -> Unit) {
        val patient = setupPatient()
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        action(OrganDonationAmendCreateFactory(patient) { registration -> amendDecision(registration) })
    }

    fun create(action: (OrganDonationAmendCreateFactory) -> Unit) {
        val patient = setupPatient()
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        action(OrganDonationAmendCreateFactory(patient) { registration -> submitDecision(registration) })
    }
}
