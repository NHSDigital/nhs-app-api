package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.set
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.junit.Assert
import worker.models.organdonation.OrganDonationSearchResponse
import worker.models.organdonation.OrganDonationState

class OrganDonationErrorStepDefinitionsBackend {

    @Given("^I am a (\\w+) api user registered with OD but Reference Data returns '(\\d+)' error$")
    fun iAmRegisteredWithOrganDonationButReferenceDataThrowsError(gpSystem: String, errorCode: Int) {
        OrganDonationFactory(gpSystem).mockingClient.forOrganDonation {
            referenceData().respondWithError(errorCode)
        }
    }

    @Given("^I am a (\\w+) api user registered with OD, but lookup returns '(\\d+)' error$")
    fun iAmRegisteredButOrganDonationWillThrowAnError(gpSystem: String, errorCode: Int) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->
            a.respondWithError(errorCode)}
    }

    @Given("^I am a (\\w+) api user who wants to opt-out, but OD returns '(\\d+)' error$")
    fun iAmAUserWhoWantsToOptInButOrganDonationButWillThrowError(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        val registrationId = "NewOrganDonationId"
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(registrationId)
        factory.create { registration -> registration.optIn { request->
            request.respondWithError(registrationId, errorCode) }}
    }

    @Given("^I am a (\\w+) user registered with OD, " +
            "but on attempting to withdraw decision OD returns (.*) error$")
    fun iAmRegisteredWithODButOnWithdrawalODThrowsRecoverableError(gpSystem: String, errorCode: Int){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.withdrawRegistration{
            request ->request.respondWithError(errorCode)
        }
    }

    @Given("^I am a (\\w+) api user amending my decision, but OD returns '(\\d+)' error$")
    fun iAmAUserWhoWantsToAmendTheirDecisionButOrganDonationThrowsError(gpSystem: String, errorCode: Int){
        val factory = OrganDonationFactory(gpSystem)
        val id = factory.existing.optInSome().id
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(id)
        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithError(id, errorCode)
            }
        }
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but organ donation will conflict$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillConflict(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->
            a.respondWithError(HttpStatus.SC_CONFLICT)}
    }

    @Given("I am a (\\w+) api user who wants to opt-in to organ donation but will cause a conflict")
    fun iAmAApiUserWhoWantsToOptInToOrganDonationButWillCauseAConflict(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val registrationId = "NewOrganDonationId"
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(registrationId)
        factory.create { registration -> registration.optIn { request->
            request.respondWithError(registrationId, HttpStatus.SC_OK) }}
    }

    @Given("I am a (\\w+) api user who wants amend their decision, but will cause a conflict")
    fun iAmAUserWhoWantsToAmendTheirOrganDonationDecisionButConflict(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        val id = factory.existing.optInSome().id
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(id)
        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithError(id, HttpStatus.SC_OK)
            }
        }
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but demographics will time out$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowTimeOutError(gpSystem: String) {
        OrganDonationFactory(gpSystem).demographicsTimeout()
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but demographics will return an internal error$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowInternalError(gpSystem: String) {
        OrganDonationFactory(gpSystem).demographicsInternalError()
    }

    @Then("^I receive an organ donation response with state value of 'conflicted'")
    fun iReceiveAnOrganDonationResponseWithStateValueOfConflicted(){
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)
        Assert.assertEquals("State",
                OrganDonationState.Conflicted,
                organDonationResponse.state)
    }
}
