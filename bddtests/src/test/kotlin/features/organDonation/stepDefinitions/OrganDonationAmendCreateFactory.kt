package features.organDonation.stepDefinitions

import mocking.MockingClient
import mocking.data.organDonation.OrganDecisions
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.models.Mapping
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.organDonation.OrganDonationSubmitDecisionBuilder
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationDemographics
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.Patient
import utils.set

class OrganDonationAmendCreateFactory(var patient: Patient,
                                      var decisionBuilder: OrganDonationMappingBuilder
                                      .(OrganDonationRegistrationRequest)-> OrganDonationSubmitDecisionBuilder) {

    private val mockingClient = MockingClient.instance

    fun optOut(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val organDonationDemographics = OrganDonationDemographics()
        OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optOut(patient,organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun optIn(demographics: OrganDonationDemographics? =null, action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val organDonationDemographics = demographics
                ?: OrganDonationDemographics(faithDeclaration = FaithDeclaration.Yes)
        OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optIn(patient, organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun some(organs : OrganDecisions,
             demographics: OrganDonationDemographics? =null,
             action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val organDonationDemographics = demographics
                ?: OrganDonationDemographics(faithDeclaration = FaithDeclaration.Yes)
        OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.some(patient, organs,
                        organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun registrationSetup(registration: OrganDonationRegistrationRequest,
                                  action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        OrganDonationSerenityHelpers.ORGAN_DONATION_DECISION.set(registration)
        mockingClient.forOrganDonation.mock { action(decisionBuilder(registration)) }
    }
}

