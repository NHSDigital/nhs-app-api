package features.organDonation.stepDefinitions

import constants.Supplier
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.models.Mapping
import mocking.organDonation.OrganDonationLookupRegistrationBuilder
import mocking.organDonation.OrganDonationSubmitWithdrawDecisionBuilder
import mocking.organDonation.models.OrganDonationWithdrawRequest
import models.Patient
import utils.SerenityHelpers

class OrganDonationFactory(val gpSystem: Supplier) {

    val mockingClient = MockingClient.instance

    val patient = setupPatient()

    val existing by lazy{OrganDonationExistingFactory(patient,gpSystem)}

    fun setupPatientForAppUse() {
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem).createFor(patient)

        mockingClient.forOrganDonation.mock {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
        }
    }

    fun lookUpRegistrationWithSuccessfulDemographics(patient: Patient? = null,
                                                     action: (OrganDonationLookupRegistrationBuilder) -> Mapping) {
        val patientToUse = patient ?: setupPatient()
        mockingClient.forOrganDonation.mock {
            action(lookupOrganDonationRegistration(patientToUse))
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patientToUse)
    }

    fun demographicsTimeout() {
        val patient = setupPatient()
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        mockingClient.forOrganDonation.mock {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabledButTimesOut(patient)
    }

    fun demographicsInternalError() {
        val patient = setupPatient()
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        mockingClient.forOrganDonation.mock {
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

    fun withdrawRegistration(action: (OrganDonationSubmitWithdrawDecisionBuilder) -> Mapping) {
        val patient = setupPatient()
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)

        val withdrawRegistration = OrganDonationWithdrawRequest.withdrawDecision(patient)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                OrganDonationSerenityHelpers.ORGAN_DONATION_WITHDRAWAL, withdrawRegistration)
        mockingClient.forOrganDonation.mock { action(withdrawOrganDonationRegistration(withdrawRegistration)) }
    }
}
