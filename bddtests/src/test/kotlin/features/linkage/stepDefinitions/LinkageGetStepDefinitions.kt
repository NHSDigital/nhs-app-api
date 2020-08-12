package features.linkage.stepDefinitions

import constants.DateTimeFormats
import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.linkage.LinkageResult
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mockingFacade.linkage.LinkageInformationFacade
import models.patients.MicrotestPatients
import net.serenitybdd.core.Serenity
import org.joda.time.DateTime
import org.junit.Assert
import utils.SerenityHelpers
import worker.models.linkage.LinkageResponse

open class LinkageGetStepDefinitions {

    private val mockingClient = MockingClient.instance

    @Given("I have valid (.*) linkage details$")
    fun iHaveValidLinkageDetailsFor(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("I have valid (.*) linkage details apart from an empty OdsCode$")
    fun iHaveValidLinkageDetailsExceptEmptyOds(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(odsCode = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details for GET but I am under (\\d+)$")
    fun iAmUnderAgeForGet(gpSystem: String, age: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0, 0, 0, 0)
        val linkage = LinkageFactory.validLinkage(supplier)
                .copy(dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone))
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientNonCompetentOrUnderMinimumAge)
    }

    @Given("^I have valid (.*) linkage details but my account status is invalid$")
    fun myAccountStatusIsInvalid(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.AccountStatusInvalid)
    }

    @Given("^I have valid (.*) linkage details but I am not found on the GP system$")
    fun iAmNotFoundOnTheGpSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.NoRegisteredOnlineUserFound)
    }

    @Given("^I have valid (.*) linkage details and it's the first time a linkage key has been retrieved for an " +
            "identity token$")
    fun itsTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrievedFirstTime)
    }

    @Given("^I have valid (.*) linkage details and it's not the first time a linkage key has been retrieved for" +
            " an identity token$")
    fun itsNotTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("^I have valid (.*) linkage details but my linkage key has been revoked$")
    fun iHaveValidLinkageDetailsButMyLinkageKeyHasBeenRevoked(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.LinkageKeyRevoked)
    }

    @Given("^(^.*) was unable to find the user for my NHS number$")
    fun wasUnableToFindTheUserForMyNHSNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.NoUserAssociatedWithNHSNumber)
    }

    @Given("^(^.*) was unable to find the api key for my NHS number$")
    fun wasUnableToFindTheApiKeyForMyNHSNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.NoApiKeyAssociatedWithNHSNumber)
    }


    @When("^I call the Linkage GET endpoint$")
    fun iCallTheLinkageGETEndpoint() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        // Only setup mock for gp supplier if we need to.
        // Some requests testing validation don't get as far as calling a gp supplier.
        if (linkage != null && linkageResult != null) {
            LinkageFactory.getForSupplier(gpSystem).mockLinkageGetResult(linkage, linkageResult)
        }
        LinkageApi.get(linkage)
    }

    @Then("^I receive a valid microtest linkage response$")
    fun iReceiveAValidMicrotestResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        val patient = MicrotestPatients.postLinkageUserDetails(
                linkageResponse.accountId, linkageResponse.linkageKey)
        DemographicsFactory.getForSupplier(Supplier.MICROTEST).enabled(patient)
        SerenityHelpers.resetPatient(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, Supplier.MICROTEST)
    }
}
