package features.linkage.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.linkage.LinkageResult
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.joda.time.DateTime
import org.junit.Assert
import utils.SerenityHelpers
import worker.models.linkage.LinkageResponse

const val DELAY: Long = 4
const val DEFAULT_TIMEOUT_MILLISECONDS: Int = 500
const val FOUR_SECOND_SLEEP: Long = 4000

open class LinkageStepDefinitions {

    val mockingClient = MockingClient.instance

    @Given("I have valid (.*) linkage details$")
    fun iHaveValidLinkageDetailsFor(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("I have valid (.*) linkage details for posting$")
    fun iHaveValidLinkageDetailsForPosting(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^another user has valid (.*) linkage details$")
    fun anotherUserHasValidLinkageDetailsFor(gpSystem: String) {
        val linkage = validOtherLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("I have valid (.*) linkage details apart from an empty OdsCode$")
    fun iHaveValidLinkageDetailsExceptEmptyOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(odsCode = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from a not found OdsCode$")
    fun iHaveValidLinkageDetailsExceptNotFoundOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(odsCode = "A04889999")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty NhsNumber$")
    fun iHaveValidLinkageDetailsExceptEmptyNhsNumber(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(nhsNumber = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty identity token$")
    fun iHaveValidLinkageDetailsExceptEmptyIdentityToken(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(identityToken = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty email address$")
    fun iHaveValidLinkageDetailsExceptEmptyEmail(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(emailAddress = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty surname$")
    fun iHaveValidLinkageDetailsExceptEmptySurname(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(surname = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty date of birth$")
    fun iHaveValidLinkageDetailsExceptEmptyDateOfBirth(gpSystem: String) {
        val linkage = validLinkage(gpSystem).copy(dateOfBirth = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details but the practice is not live$")
    fun thePracticeIsNotLive(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.PracticeNotLive)
    }

    @Given("^I have valid (.*) linkage details but the GP system has marked me as archived$")
    fun theGpSystemHasMarkedMeAsArchived(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.PatientMarkedAsArchived)
    }

    @Given("^I have valid (.*) linkage details for GET but I am under (\\d+)$")
    fun iAmUnderAgeForGet(gpSystem: String, age: Int) {
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0, 0, 0, 0)
        val linkage = validLinkage(gpSystem)
                .copy(dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone))
        setLinkageInformation(linkage, LinkageResult.PatientNonCompetentOrUnderMinimumAge)
    }

    @Given("^I have valid (.*) linkage details for POST but I am under (\\d+)$")
    fun iAmUnderAgeForPost(gpSystem: String, age: Int) {
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0, 0, 0, 0)
        val linkage = validLinkage(gpSystem)
                .copy(dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone))
        // don't need a gp supplier linkage result setup for the POST we do the age validation
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details and try to create a linkage key as (\\d+) years old$")
    fun iAmXYearsOld(gpSystem: String, age: Int) {
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).withTime(0, 0, 0, 0)
        val linkageDateOfBirthFormat = LinkageFactory.getForSupplier(gpSystem).linkageDateOfBirthFormat
        val linkage = validLinkage(gpSystem)
                .copy(dateOfBirth = dateOfBirth.toString(linkageDateOfBirthFormat))
        setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^I have valid (.*) linkage details but my account status is invalid$")
    fun myAccountStatusIsInvalid(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.AccountStatusInvalid)
    }

    @Given("^I have valid (.*) linkage details but there are multiple records for my NHS number$")
    fun thereAreMultipleRecordsForMyNhsNumber(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.MultipleRecordsFound)
    }

    @Given("^I have valid (.*) linkage details but I'm not registered at the practice$")
    fun imNotRegisteredAtThePractice(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.PatientNotRegisteredAtPractice)
    }

    @Given("^I have valid (.*) linkage details but I am not found on the GP system$")
    fun iAmNotFoundOnTheGpSystem(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.NoRegisteredOnlineUserFound)
    }

    @Given("^I have valid (.*) linkage details but the GP system responds with an internal server error " +
            "retrieving the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorRetrievingLinkageKey(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.InternalServerError)
    }

    @Given("^I have valid (.*) linkage details but the GP system responds with an internal server error " +
            "creating the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorCreatingLinkageKey(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.InternalServerError)
    }

    @Given("^I have valid (.*) linkage details and it's the first time a linkage key has been retrieved for an " +
            "identity token$")
    fun itsTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrievedFirstTime)
    }

    @Given("^I have valid (.*) linkage details and it's not the first time a linkage key has been retrieved for" +
            " an identity token$")
    fun itsNotTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("^I have valid (.*) linkage details and it's the first time a linkage key has been created for my " +
            "nhs number$")
    fun itsTheFirstTimeALinkageKeyHasBeenCreatedForMyNhsNumberAtThisPractice(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^another (.*) user has a new linkage key created for them$")
    fun anotherUserHasValidLinkageDetails(gpSystem: String) {
        val linkage = validOtherLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("I have valid (.*) linkage details but I already have an online account")
    fun iAlreadyHaveAnOnlineAccount(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.PatientAlreadyHasAnOnlineAccount)
    }

    @Given("I have valid (.*) linkage details but my nhs number is invalid")
    fun iHaveValidLinkageDetailsButMyNhsNumberIsInvalid(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.InvalidNhsNumber)
    }

    @Given("I have valid (.*) linkage details but my patient record was not found")
    fun iHaveValidLinkageDetailsButMyPatientRecordWasNotFound(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.PatientRecordNotFound)
    }

    @Given("I have valid (.*) linkage details but my linkage key has been revoked")
    fun iHaveValidLinkageDetailsButMyLinkageKeyHasBeenRevoked(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.LinkageKeyRevoked)
    }

    @Given("I have valid (.*) linkage details but a linkage key already exists")
    fun iHaveValidLinkageDetailsButALinkageKeyAlreadyExists(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.LinkageKeyAlreadyExists)
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

    @When("^I call the Linkage POST endpoint$")
    fun iCallTheLinkagePOSTEndpoint() {
        val linkage = LinkageFactory.setUpPostMocks()
        LinkageApi.post(linkage)
    }

    @When("^I call the Linkage POST endpoint which responds after (\\d+) seconds$")
    fun iCallTheLinkagePOSTEndpoint(delay: Long) {
        val linkage = LinkageFactory.setUpPostMocks(delay)
        LinkageApi.post(linkage)
    }

    @When("^I call the Linkage POST endpoint CID connection times out$")
    fun iCallTheLinkagePOSTEndpointCIDConnectionTimesOut() {
        val linkage = LinkageFactory.setUpPostMocks(DELAY)
        LinkageApi.post(linkage, DEFAULT_TIMEOUT_MILLISECONDS)
    }

    @Then("^I receive a valid linkage response$")
    fun iReceiveAValidResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        Assert.assertEquals(linkage.accountId, linkageResponse.accountId)
        Assert.assertEquals(linkage.linkageKey, linkageResponse.linkageKey)
    }

    @Then("^I receive a valid microtest linkage response$")
    fun iReceiveAValidMicrotestResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        val patient = Patient.getMicrotestPostLinkage(linkageResponse.accountId, linkageResponse.linkageKey)
        DemographicsFactory.getForSupplier("MICROTEST").enabled(patient)
        SerenityHelpers.resetPatient(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, "MICROTEST")
    }

    private fun validLinkage(gpSystem: String): LinkageInformationFacade {
        SerenityHelpers.setPatient(Patient.getDefault(gpSystem))
        SerenityHelpers.setGpSupplier(gpSystem)
        return LinkageFactory.getForSupplier(gpSystem).validLinkageDetails
    }

    private fun validOtherLinkage(gpSystem: String): LinkageInformationFacade {
        SerenityHelpers.setGpSupplier(gpSystem)
        return LinkageFactory.getForSupplier(gpSystem).validOtherLinkageDetails
    }

    private fun setLinkageInformation(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkageInformationFacade)
        Serenity.setSessionVariable(LinkageResult::class).to(linkageResult)
    }
}