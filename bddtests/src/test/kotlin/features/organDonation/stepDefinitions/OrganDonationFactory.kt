package features.organDonation.stepDefinitions

import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.models.Mapping
import mocking.organDonation.OrganDonationLookupRegistrationBuilder
import mocking.organDonation.OrganDonationSubmitDecisionBuilder
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.KeyValuePair
import models.Patient
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers

const val ORGAN_DONATION_DECISION = "ORGAN_DONATION_DECISION"
const val ORGAN_DONATION_DECISION_SOME_ORGANS = "ORGAN_DONATION_DECISION_SOME_ORGANS"
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

    fun existingOptOut() {
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun existingOptIn() {
        val registration = OrganDonationRegistrationDataBuilder.optIn(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun existingAppointedRepresentative() {
        val registration = OrganDonationRegistrationDataBuilder.appointRepresentative(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun existingOptInSome() {
        val registration = OrganDonationRegistrationDataBuilder.optInSome(patient, someOrgans())
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
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

    fun optOut(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        Serenity.setSessionVariable("ORGAN_DONATION_DECISION_OPT_IN").to(false)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optOut(patient),
                OrganDonationAdditionalDetails.fromPatient(patient))
        registrationSetup(registration, action)
    }

    fun optIn(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        Serenity.setSessionVariable("ORGAN_DONATION_DECISION_OPT_IN").to(true)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optIn(patient),
                OrganDonationAdditionalDetails.fromPatient(patient))
        registrationSetup(registration, action)
    }

    fun some(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {

        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.some(patient, someOrgans()),
                OrganDonationAdditionalDetails.fromPatient(patient))
        registrationSetup(registration, action)
    }

    private fun someOrgans(): HashMap<String, String> {
        val organsToDonate = arrayListOf(
                KeyValuePair("Heart", true),
                KeyValuePair("Lungs", false),
                KeyValuePair("Kidney", true),
                KeyValuePair("Liver", true),
                KeyValuePair("Corneas", false),
                KeyValuePair("Pancreas", true),
                KeyValuePair("Tissue", false),
                KeyValuePair("Small bowel", false))
        Serenity.setSessionVariable(ORGAN_DONATION_DECISION_SOME_ORGANS).to(organsToDonate)
        val decision = hashMapOf<String, String>()
        organsToDonate.forEach { organ -> decision.put(organ.key, if (organ.value) "yes" else "no") }
        return decision
    }

    private fun registrationSetup(registration: OrganDonationRegistrationRequest,
                                  action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val patient = setupPatient()
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        Serenity.setSessionVariable(ORGAN_DONATION_DECISION).to(registration)
        mockingClient.forOrganDonation { action(submitDecision(registration)) }
    }

    private fun setupPatient(): Patient {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        return patient
    }
}
