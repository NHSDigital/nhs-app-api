package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.organDonation.ORGAN_DONATION_ERROR_CODE_UPDATE_CONFLICT
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.KeyValuePair
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.OrganDonationDemographics
import utils.SerenityHelpers

open class OrganDonationAmendStepDefinitions {

    @Given("I am a (.*) user registered as opt-in who then amends their decision to opt-out")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptIn()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.optOut {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as opt-in who then amends their decision to some")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToSome(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptIn()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.some {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as opt-in who then amends their faith and beliefs")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendMyFaithAndBeliefs(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()

        val faithDeclaration = OrganDonationDemographics(faithDeclaration = FaithDeclaration.NotStated)
        val existingRegistration = factory.existingOptIn(faithDeclaration)
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        val amendedDemographics =
                OrganDonationDemographics(
                        faithDeclaration = FaithDeclaration.Yes,
                        ethnicity = KeyValuePair(OrganDonationReferenceDataBuilder.chinese.code,
                                OrganDonationReferenceDataBuilder.chinese.display),
                        religion = KeyValuePair(OrganDonationReferenceDataBuilder.hindhu.code,
                                OrganDonationReferenceDataBuilder.hindhu.display))

        val amendedRequest = OrganDonationRegistrationRequest(
                registration = OrganDonationRegistration.optIn(SerenityHelpers.getPatient(), amendedDemographics),
                additionalDetails = OrganDonationAdditionalDetails.getAdditionalDetails(amendedDemographics)
        )

        factory.amend { registration ->
            registration.registrationSetup(amendedRequest) {
                request -> request.respondWithSuccess(existingRegistration.id) }
        }
    }

    @Given("I am a (.*) user registered as some who then amends their decision to opt-out")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptInSome()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.optOut {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as some who then amends their decision to opt-in")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptInSome()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.optIn {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as some who then amends their selected organs")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendTheSelectedOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptInSome()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.some {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as opt-out who then amends their decision to opt-in")
    fun iAmRegisteredWithOrganDonationAsOptOutButAmendToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptOut()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.optIn {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as opt-out who then amends their decision to some")
    fun iAmRegisteredWithOrganDonationAsOptOutButAmendToSome(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptOut()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration->registration.some {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }


    @Given("I am a (.*) user registered as opt-in with organ donation, " +
            "who wishes to opt-out but will cause a conflict")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToOutAndCauseConflict(gpSystem: String) {

        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existingOptIn()
        OrganDonationSerenityHelpers.setRegistrationId(existingRegistration.id)

        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithConflict(existingRegistration.id,
                       ORGAN_DONATION_ERROR_CODE_UPDATE_CONFLICT.toString())
            }
        }
    }
}
