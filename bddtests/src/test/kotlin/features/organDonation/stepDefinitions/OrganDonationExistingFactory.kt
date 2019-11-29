package features.organDonation.stepDefinitions

import constants.Supplier
import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import features.myrecord.factories.DemographicsFactory
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.OrganDonationDemographics
import mocking.organDonation.models.Resource
import models.Patient

class OrganDonationExistingFactory (var patient: Patient, val gpSystem: Supplier){

    fun optOut(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient, organDonationDemographics)
        existing(registration)
        return registration
    }

    fun optIn(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optIn(patient,
                organDonationDemographics ?: OrganDonationDemographics(faithDeclaration = FaithDeclaration.No))
        existing(registration)
        return registration
    }

    fun appointedRepresentative() : Resource {
        val registration = OrganDonationRegistrationDataBuilder.appointRepresentative(patient)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        return registration
    }

    fun optInSome(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optInSome(patient,
                organDonationDemographics ?: OrganDonationDemographics(faithDeclaration = FaithDeclaration.No))
        existing(registration)
        return registration
    }

    fun optInSomeNotAllDecided(organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val registration = OrganDonationRegistrationDataBuilder.optInSomeNotAllDecided(patient,
                organDonationDemographics ?:  OrganDonationDemographics(faithDeclaration = FaithDeclaration.No))
        existing(registration)
        return registration
    }

    fun setUpExistingDecisionForPatient(decision: String): Resource{
        val factoryActions: MutableMap<String, () -> Resource > = mutableMapOf(
                "opt-in" to {optIn()},
                "opt-out" to {optOut()},
                "opt-in-some" to {optInSome()},
                "appoint-a-representative" to {appointedRepresentative()}
        )
        return factoryActions.get(decision)!!.invoke()
    }

    private fun existing(registration: Resource) {
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess(registration)
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }
}