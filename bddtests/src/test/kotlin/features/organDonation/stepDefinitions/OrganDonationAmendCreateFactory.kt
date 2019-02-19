package features.organDonation.stepDefinitions

import mocking.MockingClient
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.set
import mocking.models.Mapping
import mocking.organDonation.OrganDonationMappingBuilder
import mocking.organDonation.OrganDonationSubmitDecisionBuilder
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationDemographics
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.Patient

class OrganDonationAmendCreateFactory(var patient: Patient,
                                      var decisionBuilder: OrganDonationMappingBuilder
                                      .(OrganDonationRegistrationRequest)-> OrganDonationSubmitDecisionBuilder) {

    val mockingClient = MockingClient.instance

    fun optOut(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        OrganDonationSerenityHelpers.IS_OPT_IN.set(false)
        val organDonationDemographics = OrganDonationDemographics()
        OrganDonationSerenityHelpers.DEMOGRAPHICS.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optOut(patient,organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun optIn(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        OrganDonationSerenityHelpers.IS_OPT_IN.set(true)
        val organDonationDemographics = OrganDonationDemographics()
        OrganDonationSerenityHelpers.DEMOGRAPHICS.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.optIn(patient,organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun some(action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        val organDonationDemographics = OrganDonationDemographics()
        OrganDonationSerenityHelpers.DEMOGRAPHICS.set(organDonationDemographics)
        val registration = OrganDonationRegistrationRequest(
                OrganDonationRegistration.some(patient, OrganDonationRegistrationDataBuilder.someOrgansListUpdated(),
                        organDonationDemographics),
                OrganDonationAdditionalDetails.getAdditionalDetails(organDonationDemographics))
        registrationSetup(registration, action)
    }

    fun registrationSetup(registration: OrganDonationRegistrationRequest,
                                  action: (OrganDonationSubmitDecisionBuilder) -> Mapping) {
        OrganDonationSerenityHelpers.ORGAN_DONATION_DECISION.set(registration)
        mockingClient.forOrganDonation { action(decisionBuilder(registration)) }
    }
}

