package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.set

class OrganDonationAmendStepDefinitionsBackend {

    @Given("I am a (\\w+) api user registered as opt-out who amends their decision to opt-in")
    fun iAmARegisteredWithOrganDonationButWantToAmendDecisionToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val existingRegistration = factory.existingOptOut()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.optIn {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) api user registered as opt-in who amends their decision to some organs")
    fun iAmARegisteredWithOrganDonationButWantToAmendDecisionToSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val existingRegistration = factory.existingOptIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration->registration.some(OrganDonationRegistrationDataBuilder.someOrgansListUpdated()) {
            request -> request.respondWithSuccess(existingRegistration.id) }}
    }

    @Given("I am a (\\w+) api user registered as some organs who amend their decision to opt-out")
    fun iAmARegisteredWithOrganDonationButWantToAmendDecisionToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val existingRegistration = factory.existingOptInSome()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithSuccess(existingRegistration.id)
            }
        }
    }
}
