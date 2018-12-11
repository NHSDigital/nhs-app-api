package features.linkage.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.linkage.LinkageResult
import mocking.MockingClient
import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import org.joda.time.DateTime
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

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

    @Given("I have valid (.*) linkage details apart from an empty OdsCode$")
    fun iHaveValidLinkageDetailsExceptEmptyOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.odsCode = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from a not found OdsCode$")
    fun iHaveValidLinkageDetailsExceptNotFoundOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.odsCode = "A04889999"
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty NhsNumber$")
    fun iHaveValidLinkageDetailsExceptEmptyNhsNumber(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.nhsNumber = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty identity token$")
    fun iHaveValidLinkageDetailsExceptEmptyIdentityToken(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.identityToken = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty email address$")
    fun iHaveValidLinkageDetailsExceptEmptyEmail(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.emailAddress = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty surname$")
    fun iHaveValidLinkageDetailsExceptEmptySurname(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.surname = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty date of birth$")
    fun iHaveValidLinkageDetailsExceptEmptyDateOfBirth(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.dateOfBirth = ""
        setLinkageInformation(linkage)
    }

    @Given("^I have valid (.*) linkage details but the practice is not live$")
    fun thePraticeIsNotLive(gpSystem: String) {
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
        val linkage = validLinkage(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0,0,0,0)
        linkage.dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        setLinkageInformation(linkage, LinkageResult.PatientNonCompetentOrUnderMinimumAge)
    }

    @Given("^I have valid (.*) linkage details for POST but I am under (\\d+)$")
    fun iAmUnderAgeForPost(gpSystem: String, age: Int) {
        val linkage = validLinkage(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0,0,0,0)
        linkage.dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone)

        // don't need a gp supplier linkage result setup for the POST we do the age validation
        setLinkageInformation(linkage)
    }

    @Given("^I have valid (.*) linkage details and try to create a linkage key as (\\d+) years old$")
    fun iAmXYearsOld(gpSystem: String, age: Int) {
        val linkage = validLinkage(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).withTime(0,0,0,0)
        val linkageDateOfBirthFormat = LinkageFactory.getForSupplier(gpSystem).linkageDateOfBirthFormat
        linkage.dateOfBirth = dateOfBirth.toString(linkageDateOfBirthFormat)
        setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^I have valid (.*) linkage details but my account status is invalid$")
    fun myAccountStatusIsInvalid(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage, LinkageResult.AccountStatusInvalid)
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

    @When("^I call the (.*) Linkage GET endpoint$")
    fun iCallTheLinkageGETEndpoint(gpSystem: String) {

        val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        // Only setup mock for gp supplier if we need to.
        // Some requests testing validation don't get as far as calling a gp supplier.
        if (linkage != null && linkageResult != null) {
            LinkageFactory.getForSupplier(gpSystem).mockLinkageGetResult(linkage, linkageResult)
        }

        try {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getLinkageKey(linkage)

            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("I call the (.*) Linkage POST endpoint")
    fun iCallTheLinkagePOSTEndpoint(gpSystem: String) {

        val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        // Only setup mock for gp supplier if we need to.
        // Some requests testing validation don't get as far as calling a gp supplier.
        if (linkage != null && linkageResult != null) {
            LinkageFactory.getForSupplier(gpSystem).mockLinkagePostResult(linkage, linkageResult)
        }

        try {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postLinkageKey(CreateLinkageRequest(
                    linkage.odsCode,
                    linkage.nhsNumber,
                    linkage.identityToken,
                    linkage.emailAddress,
                    linkage.dateOfBirth,
                    linkage.surname))

            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I receive a valid linkage response$")
    fun iReceiveAValidResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        Assert.assertEquals(linkage.accountId, linkageResponse.accountId)
        Assert.assertEquals(linkage.linkageKey, linkageResponse.linkageKey)
    }

    private fun validLinkage(gpSystem: String): LinkageInformationFacade {
        return LinkageFactory.getForSupplier(gpSystem).validLinkageDetails
    }

    private fun setLinkageInformation(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkageInformationFacade)
        Serenity.setSessionVariable(LinkageResult::class).to(linkageResult)
    }

    private fun setLinkageInformation(linkageInformationFacade: LinkageInformationFacade) {
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkageInformationFacade)
    }
}


