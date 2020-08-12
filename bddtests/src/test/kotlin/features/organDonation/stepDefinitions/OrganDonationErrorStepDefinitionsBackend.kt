package features.organDonation.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.set
import utils.SerenityHelpers
import worker.models.organdonation.OrganDonationSearchResponse
import worker.models.organdonation.OrganDonationState

class OrganDonationErrorStepDefinitionsBackend {

    @Given("^I am a (\\w+) api user registered with OD but Reference Data returns '(\\d+)' error$")
    fun iAmRegisteredWithOrganDonationButReferenceDataThrowsError(gpSystem: String, errorCode: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        OrganDonationFactory(supplier).mockingClient.forOrganDonation.mock {
            referenceData().respondWithError(errorCode)
        }
    }

    @Given("^I am a (\\w+) api user registered with OD, but lookup returns '(\\d+)' error$")
    fun iAmRegisteredButOrganDonationWillThrowAnError(gpSystem: String, errorCode: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        OrganDonationFactory(supplier).lookUpRegistrationWithSuccessfulDemographics{ a->
            a.respondWithError(errorCode)}
    }

    @Given("^I am a (\\w+) api user who wants to opt-out, but OD returns '(\\d+)' error$")
    fun iAmAUserWhoWantsToOptInButOrganDonationButWillThrowError(gpSystem: String, errorCode: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        val registrationId = "NewOrganDonationId"
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(registrationId)
        factory.create { registration -> registration.optIn { request->
            request.respondWithError(registrationId, errorCode) }}
    }

    @Given("^I am a (\\w+) user registered with OD, " +
            "but on attempting to withdraw decision OD returns (.*) error$")
    fun iAmRegisteredWithODButOnWithdrawalODThrowsRecoverableError(gpSystem: String, errorCode: Int){
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        factory.withdrawRegistration{
            request ->request.respondWithError(errorCode)
        }
    }

    @Given("^I am a (\\w+) api user amending my decision, but OD returns '(\\d+)' error$")
    fun iAmAUserWhoWantsToAmendTheirDecisionButOrganDonationThrowsError(gpSystem: String, errorCode: Int){
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
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
        val supplier = Supplier.valueOf(gpSystem)
        OrganDonationFactory(supplier).lookUpRegistrationWithSuccessfulDemographics{ a->
            a.respondWithError(HttpStatus.SC_CONFLICT)}
    }

    @Given("^I am a (\\w+) api user who wants to opt-in to organ donation but will cause a conflict$")
    fun iAmAApiUserWhoWantsToOptInToOrganDonationButWillCauseAConflict(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        val registrationId = "NewOrganDonationId"
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(registrationId)
        factory.create { registration -> registration.optIn { request->
            request.respondWithError(registrationId, HttpStatus.SC_OK) }}
    }

    @Given("^I am a (\\w+) api user who wants amend their decision, but will cause a conflict$")
    fun iAmAUserWhoWantsToAmendTheirOrganDonationDecisionButConflict(gpSystem: String){
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        val id = factory.existing.optInSome().id
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(id)
        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithError(id, HttpStatus.SC_OK)
            }
        }
    }

    @Given("^I am an api user registered with organ donation, but demographics will time out$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowTimeOutError() {
        OrganDonationFactory(SerenityHelpers.getGpSupplier()).demographicsTimeout()
    }

    @Given("^I am an api user registered with organ donation, but demographics will return an internal error$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowInternalError() {
        OrganDonationFactory(SerenityHelpers.getGpSupplier()).demographicsInternalError()
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
