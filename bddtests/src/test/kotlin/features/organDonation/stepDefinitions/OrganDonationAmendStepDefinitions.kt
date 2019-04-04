package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.KeyValuePair
import mocking.organDonation.models.OrganDonationAdditionalDetails
import mocking.organDonation.models.OrganDonationRegistration
import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.OrganDonationDemographics
import mocking.organDonation.ORGAN_DONATION_ERROR_CODE_UPDATE_CONFLICT
import utils.SerenityHelpers
import utils.set

open class OrganDonationAmendStepDefinitions {

    @Given("I am a (\\w+) user registered as opt-in who then amends their decision to opt-out")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optOut {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as opt-in who then amends their decision to some")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToSome(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = FaithDeclaration.No)
        val existingRegistration = factory.existing.optIn(demographics)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.some(
                OrganDonationRegistrationDataBuilder.someOrgansListUpdated(), demographics) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as opt-in who then amends their faith and beliefs")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendMyFaithAndBeliefs(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()

        val originalDemographics = OrganDonationDemographics(faithDeclaration = FaithDeclaration.No)
        val existingRegistration = factory.existing.optIn(originalDemographics)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        val amendedDemographics =
                OrganDonationDemographics(
                        faithDeclaration = FaithDeclaration.Yes,
                        ethnicity = KeyValuePair(OrganDonationReferenceDataBuilder.chinese.code,
                                OrganDonationReferenceDataBuilder.chinese.display),
                        religion = KeyValuePair(OrganDonationReferenceDataBuilder.hindhu.code,
                                OrganDonationReferenceDataBuilder.hindhu.display))
        OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED.set(amendedDemographics)

        val amendedRequest = OrganDonationRegistrationRequest(
                registration = OrganDonationRegistration.optIn(SerenityHelpers.getPatient(), amendedDemographics),
                additionalDetails = OrganDonationAdditionalDetails.getAdditionalDetails(amendedDemographics)
        )

        factory.amend { registration ->
            registration.registrationSetup(amendedRequest) {
                request -> request.respondWithSuccess(existingRegistration.id) }
        }
    }

    @Given("I am a (\\w+) user registered as some who then amends their decision to opt-out")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optInSome()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optOut() {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as some who then amends their decision to opt-in")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = FaithDeclaration.Yes)
        val existingRegistration = factory.existing.optInSome(demographics)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optIn(demographics) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as some who then amends their selected organs")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendTheSelectedOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = FaithDeclaration.Yes)
        val existingRegistration = factory.existing.optInSome(demographics)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.some(
                OrganDonationRegistrationDataBuilder.someOrgansListUpdated(), demographics) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered to donate some organs, with some undecided, who amends their decision")
    fun iAmRegisteredWithOrganDonationAsSomeButAmendTheSelectedOrgansToDecideUndecided(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val demographics = OrganDonationDemographics(faithDeclaration = FaithDeclaration.Yes)
        val existingRegistration = factory.existing.optInSomeNotAllDecided(demographics)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration ->
            registration.some(
                    OrganDonationRegistrationDataBuilder.someOrgansListUpdated(), demographics)
            { request -> request.respondWithSuccess(existingRegistration.id) }
        }
    }

    @Given("I am a (\\w+) user registered as opt-out who then amends their decision to opt-in")
    fun iAmRegisteredWithOrganDonationAsOptOutButAmendToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optOut()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optIn {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as opt-out who then amends their decision to some")
    fun iAmRegisteredWithOrganDonationAsOptOutButAmendToSome(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optOut()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.some(
                OrganDonationRegistrationDataBuilder.someOrgansListUpdated()) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (.*) user registered as opt-in with organ donation, who wishes to opt-out but will cause " +
            "a conflict")
    fun iAmRegisteredWithOrganDonationAsOptInButAmendToOutAndCauseConflict(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithConflict(existingRegistration.id,
                        ORGAN_DONATION_ERROR_CODE_UPDATE_CONFLICT.toString())
            }
        }
    }

    @Given("I am a (\\w+) user registered as opt-in with organ donation, who wishes to amend$")
    fun iAmUserRegisteredWithOrganDonationWhoWishesToAmend(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.optIn()
    }

    @Given("I am a (\\w+) user registered as opt-in who wishes to reaffirm their decision")
    fun iAmAUserRegisteredAsOptInWhoWishesToReaffirmTheirDecision(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optIn {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as opt-out who wishes to reaffirm their decision")
    fun iAmAUserRegisteredAsOptOutWhoWishesToReaffirmTheirDecision(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optOut()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optOut {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) user registered as opt-in with some organs who wishes to reaffirm their decision")
    fun iAmAUserRegisteredAsOptInWithSomeOrgansWhoWishesToReaffirmTheirDecision(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optInSome()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.some(
                OrganDonationRegistrationDataBuilder.someOrgansListUpdated()) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }
}
